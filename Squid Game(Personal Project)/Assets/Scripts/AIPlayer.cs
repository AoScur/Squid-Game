using Newtonsoft.Json.Bson;
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
    public LayerMask whatIsTarget;
    private Transform target;

    private Transform goalLinePos;

    private Animator animator;

    private float distanceToGoalLine;
    private float distanceToTarget;
    public float pushRange = 3f;
    public float timer = 0f;
    public float chaseInterval = 2f;
    public bool run = false;
    public float proximate = 0f;
    public float coolDown = 0.25f;

    private List<LivingEntity> targets;

    [HideInInspector]
    public AIData aiData;
    public AIType type { get; private set; } // 현재 AI 종류

    private States state = States.None;

    public States State
    {
        get { return state; }
        private set
        {
            var prevState = state;
            state = value;

            Vector3 runDestination = transform.position;
            runDestination.z = goalLinePos.position.z;

            if (prevState == state)
                return;

            switch (state)
            {
                case States.Idle:
                    timer = 0f;
                    agent.enabled = true;
                    agent.isStopped = true;
                    agent.enabled = false;
                    animator.SetBool("Push", false);

                    //animator.SetTrigger("Idle");
                    //Debug.Log("Idle");
                    break;
                case States.Run:
                    agent.enabled = true;
                    agent.isStopped = false;
                    rb.isKinematic = true;
                    agent.SetDestination(runDestination);
                    agent.speed = speed;
                    animator.SetBool("Push", false);

                    //animator.SetTrigger("Run");
                    //Debug.Log("Run");
                    break;
                case States.Chase:
                    timer = 0f;
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
                            target = collider.transform;
                        }
                    }
                    if (target == null)
                    {
                        State = States.Run;
                        return;
                    }
                    agent.SetDestination(target.position);
                    agent.speed = chaseSpeed;
                    break;
                case States.Push:
                    agent.enabled = true;
                    agent.isStopped = true;
                    agent.enabled = false;
                    var lookPos = target.position;
                    lookPos.y = transform.position.y;
                    transform.LookAt(lookPos);
                    animator.SetBool("Push", true);
                    //Debug.Log("Push");
                    break;
                case States.Die:
                    animator.SetBool("Push", false);
                    agent.enabled = true;
                    agent.isStopped = true;
                    agent.enabled = false;
                    break;
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        goalLinePos = GameObject.FindWithTag("GoalLine").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        targets = AIPlayerSpawner.targets;
        //Debug.Log(aiData.name);
    }

    private void Start()
    {
        State = States.Idle;

        StartCoroutine(StateChange());
    }

    //private void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying)
    //        return;
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, 10f);
    //}

    private void Update()
    {
        if (target != null)
        {
            if (State != States.Die)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);
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
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private IEnumerator StateChange()
    {
        while (!dead)
        {
            switch (type)
            {
                case AIType.Heavy:
                    {
                        if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 70)
                        {
                            RandomBehavior();
                            //Debug.Log("해비랜덤1");
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                        {
                            RandomBehavior();
                            //Debug.Log("해비랜덤2");
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else
                        {
                            RunOnlyBehavior();
                            //Debug.Log("해비런온니");
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                    }
                case AIType.Speedy:
                    {
                        if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 70)
                        {
                            RunOnlyBehavior();
                            //Debug.Log("스피디런온니1");
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                        {
                            RandomBehavior();
                            //Debug.Log("스피디랜덤");
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else
                        {
                            RunOnlyBehavior();
                            //Debug.Log("스피디런온니2");
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                    }
                case AIType.Strongy:
                    {
                        if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 70)
                        {
                            FightFirstBehavior();
                            //Debug.Log("스트롱기파이트퍼스트");
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                        {
                            RandomBehavior();
                            //Debug.Log("스트롱기랜덤");
                            yield return new WaitForSeconds(Random.value * 3f);
                            break;
                        }
                        else
                        {
                            RunOnlyBehavior();
                            //Debug.Log("스트롱기런온니");
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
        float[] probs = { 60, 40 };

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
        float[] probs = { 60, 25, 15 };

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
        float[] probs = { 20, 10, 70 };

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

    private void UpdateChase()
    {
        timer += Time.deltaTime;

        if (target == null)
        {
            State = States.Run;
            return;
        }

        if (distanceToTarget < pushRange )
        {
            State = States.Push;
            return;
        }
        if (timer > chaseInterval)
        {
            agent.SetDestination(target.position);
            timer = 0f;
        }
    }

    private void UpdatePush()
    {
        timer += Time.deltaTime;

        if (distanceToTarget > pushRange)
        {
            State = States.Chase;
            return;
        }
    }

    public override void OnPush(float strength, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (dead)
        {
            return;
        }

        agent.isStopped = true;
        agent.enabled = false;
        animator.SetTrigger("OnPush");
        base.OnPush(strength, hitPoint, hitNormal);
        Debug.Log("OnPush");
    }

    public override void OnDie(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (dead)
        {
            return;
        }

        //hurtEffect.transform.position = hitPoint;
        //hurtEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
        //hurtEffect.Play();

        //zomAudioPlayer.PlayOneShot(hurtClip);
        base.OnDie(hitPoint, hitNormal);

        animator.SetTrigger("Die");

        var colliders = GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
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

    public void SearchTarget()
    {
    }
}
