using Newtonsoft.Json.Bson;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


public class AIPlayer : LivingEntity
{
    public enum AIType
    {
        None = -1,
        Heavy,
        Speedy,
        Strongy,
    }



    private List<GameObject> Alives = new List<GameObject>();
    private Transform target;

    private Transform goalLinePos;

    private Animator animator;

    private float distanceToGoalLine;
    private float distanceToTarget;
    public float pushRange;
    private float timer = 0f;
    public float chaseInterval = 0.25f;
    private bool run = false;
    private int index = 0;

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
                    agent.isStopped = true;
                    break;
                case States.Run:
                    agent.speed = speed;
                    agent.isStopped = false;
                    agent.SetDestination(runDestination);
                    break;
                case States.Chase:
                    timer = 0f;
                    agent.speed = chaseSpeed;
                    agent.isStopped = false;
                    break;
                case States.Push:
                    agent.isStopped = true;
                    break;
                case States.Die:
                    agent.isStopped = true;
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

    private void Update()
    {

    }

    private IEnumerator StateChange()
    {
        while (true)
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
    }


    private void RunOnlyBehavior()
    {
        float[] probs = { 60, 40 };

        switch (RandomNumber(probs))
        {
            case ((int)States.Idle):
                UpdateIdle();
                break;
            case ((int)States.Run):
                UpdateRun();
                break;
        }
    }

    private void RandomBehavior()
    {
        //float[] probs = { 60, 25, 15 };
        float[] probs = { 10, 10, 80 };

        switch (RandomNumber(probs))
        {
            case ((int)States.Idle):
                UpdateIdle();
                break;
            case ((int)States.Run):
                UpdateRun();
                break;
            case ((int)States.Chase):
                UpdateChase();
                break;
        }
    }

    private void FightFirstBehavior()
    {
        float[] probs = { 20, 10, 70 };

        switch (RandomNumber(probs))
        {
            case ((int)States.Idle):
                UpdateIdle();
                break;
            case ((int)States.Run):
                UpdateRun();
                break;
            case ((int)States.Chase):
                UpdateChase();
                break;
        }
    }

    private void UpdateIdle()
    {
        State = States.Idle;
        animator.SetTrigger("Idle");
    }

    private void UpdateRun()
    {
        State = States.Run;
        animator.SetTrigger("Run");
        rb.isKinematic = true;
        agent.destination = goalLinePos.position;
        agent.speed = speed;
    }

    private void UpdateChase()
    {
        timer += Time.deltaTime;
        index = Random.Range(0, targets.Count);
        target = targets[index].transform;
        if (distanceToTarget < pushRange)
        {
            State = States.Push;
            return;
        }
        if (timer > chaseInterval)
        {
            State = States.Chase;
            agent.SetDestination(target.position);
            timer = 0f;

            Debug.Log("Chase");
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

        timer = 0f;
        var lookPos = target.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        animator.SetBool("Push", true);
        Debug.Log("Push");
        State = States.Run;
    }

    public override void OnPush(float strength, Vector3 hitPoint, Vector3 hitNormal)
    {

        base.OnPush(strength, hitPoint, hitNormal);
        agent.isStopped = true;
        agent.enabled = false;

        animator.SetTrigger("OnPush");
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
        agent.isStopped = true;
        agent.enabled = false;

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
