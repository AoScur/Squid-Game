using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


public class AIPlayer : LivingEntity
{
    public enum AIType
    {
        None = -1,
        Heavy,
        Speedy,
        Strongy,
    }

    public NavMeshAgent agent;

    public LayerMask whatIsTarget;
    private LivingEntity target;

    private Transform goalLinePos;
    private Transform destinationPos;

    private float distanceToTarget;
    public float pushRange = 1f;
    public float timer = 0f;
    public float chaseInterval = 2f;
    public bool run = false;
    public float proximate = 0f;
    public float coolDown = 0.25f;

    Coroutine roofCoroutine = null;
    bool skip = false;
    bool isGameOver = false;

    [HideInInspector]
    public AIData aiData;
    public AIType type { get; private set; } // 현재 AI 종류

    public override States State
    {
        get { return state; }
        set
        {
            var prevState = state;
            state = value;

            Vector3 runDestination = transform.position;
            runDestination.z = destinationPos.position.z;

            if (prevState == state)
                return;


            switch (state)
            {
                case States.Idle:
                    timer = 0f;
                    rb.isKinematic = true;
                    agent.enabled = true;
                    agent.isStopped = true;
                    agent.enabled = false;
                    animator.SetBool("Push", false);
                    break;
                case States.Run:
                    rb.isKinematic = true;
                    agent.enabled = true;
                    agent.isStopped = false;
                    rb.isKinematic = true;
                    agent.SetDestination(runDestination);
                    agent.speed = speed;
                    animator.SetBool("Push", false);
                    break;
                case States.Chase:
                    timer = 0f;
                    rb.isKinematic = true;
                    agent.enabled = true;
                    agent.isStopped = false;
                    animator.SetBool("Push", false);

                    var colliders = Physics.OverlapSphere(transform.position, 10f, whatIsTarget);
                    foreach (var collider in colliders)
                    {
                        if (proximate == 0)
                        {
                            proximate = Vector3.Distance(transform.position, collider.transform.position);
                        }
                        if (proximate >= Vector3.Distance(transform.position, collider.transform.position))
                        {
                            if (this.gameObject == collider.gameObject)
                                continue;

                            proximate = Vector3.Distance(transform.position, collider.transform.position);
                            target = collider.gameObject.GetComponent<LivingEntity>();
                        }
                    }
                    if (target == null)
                    {
                        State = States.Run;
                        return;
                    }
                    agent.SetDestination(target.transform.position);
                    agent.speed = chaseSpeed;
                    break;
                case States.Push:
                    skip = true;
                    rb.isKinematic = true;
                    agent.enabled = true;
                    agent.isStopped = true;
                    agent.enabled = false;

                    var lookPos = target.transform.position;
                    lookPos.y = transform.position.y;
                    transform.LookAt(lookPos);

                    animator.SetBool("Push", true);
                    break;
                case States.OnPush:
                    skip = true;
                    agent.enabled = true;
                    agent.isStopped = true;
                    agent.enabled = false;
                    rb.isKinematic = false;
                    animator.SetBool("Push", false);
                    break;
                case States.Die:
                    rb.isKinematic = true;
                    animator.SetBool("Push", false);
                    agent.enabled = true;
                    agent.isStopped = true;
                    agent.enabled = false;
                    break;
            }
        }
    }

    public override void Awake()
    {
        goalLinePos = GameObject.FindWithTag("GoalLine").GetComponent<Transform>();
        destinationPos = GameObject.FindWithTag("Destination").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        base.Awake();
    }

    private void Start()
    {
        GameManager.instance.onGameOver += EndGame;
        State = States.Idle;
        StartStateChange();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("GoalLine"))
        {
            Invoke("CrossGoalLine", 1f);
        }
    }

    private void CrossGoalLine()
    {
        StopStateChange();
        State = States.Idle;
    }

    private void Update()
    {
        if (isGameOver)
            return;

        if (target != null)
        {
            if (State != States.Die)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            }
        }

        switch (State)
        {
            case States.Chase:
                UpdateChase();
                break;
            case States.Push:
                UpdatePush();
                break;
            case States.OnPush:
                UpdateOnPush();
                break;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void UpdateChase()
    {
        timer += Time.deltaTime;

        if (target == null)
        {
            State = States.Run;
            return;
        }

        if (distanceToTarget < pushRange)
        {
            State = States.Push;
            return;
        }
        if (timer > chaseInterval)
        {
            agent.SetDestination(target.transform.position);
            timer = 0f;
        }
    }


    private void UpdatePush()
    {
        if (animator.GetBool("Push"))
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Push") && info.normalizedTime >= 1f)
            {
                animator.SetBool("Push", false);
                skip = false;
            }
        }
    }

    private void UpdateOnPush()
    {
        if (animator.GetBool("OnPush"))
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("OnPush") && info.normalizedTime >= 1f)
            {
                animator.SetBool("OnPush", false);
                skip = false;
            }
        }
    }

    private void StartStateChange()
    {
        roofCoroutine = StartCoroutine(StateChange());
    }

    private void StopStateChange()
    {
        if (roofCoroutine != null)
        {
            StopCoroutine(roofCoroutine);
        }
    }

    private IEnumerator StateChange()
    {
        while (!dead)
        {
            if (skip)
            {
                yield return null;
                continue;
            }
            switch (type)
            {
                case AIType.Heavy:
                    {
                        if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 70)
                        {
                            RandomBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                        {
                            RandomBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else
                        {
                            RunOnlyBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                    }
                case AIType.Speedy:
                    {
                        if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 70)
                        {
                            RunOnlyBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                        {
                            RandomBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else
                        {
                            RunOnlyBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                    }
                case AIType.Strongy:
                    {
                        if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 70)
                        {
                            FightFirstBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                        {
                            RandomBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else
                        {
                            RunOnlyBehavior();
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                    }
            }
        }
    }

    public void Setup(AIData data)
    {
        speed = data.speed;
        mass = data.mass;
        power = data.power;
        type = (AIType)data.aiType;
        chaseSpeed = data.speed * 1.5f;
    }


    private void RunOnlyBehavior()
    {
        float[] probs = { 70, 30 };

        switch (RandomNumber(probs))
        {
            case ((int)States.Idle):
                State = States.Idle;
                break;
            case ((int)States.Run):
                State = States.Run;
                break;
        }
    }

    private void RandomBehavior()
    {
        float[] probs = { 70, 20, 10 };

        switch (RandomNumber(probs))
        {
            case ((int)States.Idle):
                State = States.Idle;
                break;
            case ((int)States.Run):
                State = States.Run;
                break;
            case ((int)States.Chase):
                State = States.Chase;
                break;
        }
    }

    private void FightFirstBehavior()
    {
        float[] probs = { 60, 10, 30 };

        switch (RandomNumber(probs))
        {
            case ((int)States.Idle):
                State = States.Idle;
                break;
            case ((int)States.Run):
                State = States.Run;
                break;
            case ((int)States.Chase):
                State = States.Chase;
                break;
        }
    }

    public void Kick()
    {
        var hitPoint = rightBall.GetComponent<CapsuleCollider>().ClosestPoint(target.transform.position);
        var hitNormal = (transform.position - target.transform.position).normalized;


        target.OnPush(hitPoint, hitNormal);

    }

    public override void OnPush(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (dead)
        {
            return;
        }
        State = States.OnPush;
        animator.SetBool("OnPush", true);
        base.OnPush(hitPoint, hitNormal);
    }

    public override void OnDie(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (dead)
        {
            return;
        }
        StopStateChange();
        State = States.Die;
        animator.SetTrigger("Die");
        base.OnDie(hitPoint, hitNormal);
    }

    public float RandomNumber(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    private void EndGame()
    {
        isGameOver = true;        
        StopStateChange();
        if (State == States.Die || State == States.OnPush)
            State = States.Idle;
    }
}
