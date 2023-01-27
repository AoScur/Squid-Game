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
    private bool run = true;

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
                    agent.SetDestination(target.position);
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
        agent = GetComponent<NavMeshAgent>();
        goalLinePos = GameObject.FindWithTag("GoalLine").GetComponent<Transform>();
    }

    private void Start()
    {
        State = States.Idle;


    }

    private void Update()
    {
        switch (type)
        {
            case AIType.Heavy:
                {
                    if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 100)
                    {
                        RandomBehavior();
                        break;
                    }
                    else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                    {
                        RandomBehavior();
                        break;
                    }
                    else
                    {
                        RunOnlyBehavior();
                        break;
                    }
                }
            case AIType.Speedy:
                {
                    if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 100)
                    {
                        RunOnlyBehavior();
                        break;
                    }
                    else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                    {
                        RandomBehavior();
                        break;
                    }
                    else
                    {
                        RunOnlyBehavior();
                        break;
                    }
                }
            case AIType.Strongy:
                {
                    if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 100)
                    {
                        FightFirstBehavior();
                        break;
                    }
                    else if (Mathf.Abs(goalLinePos.position.z - transform.position.z) > 30)
                    {
                        RandomBehavior();
                        break;
                    }
                    else
                    {
                        RunOnlyBehavior();
                        break;
                    }
                }
        }
    }

    public void Setup(AIData data)
    {
        speed = data.speed;
        mass = data.mass;
        power = data.power;
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
        float[] probs = { 60, 25, 15 };
        
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
        animator.SetTrigger("Idle");
    }

    private void UpdateRun()
    {
        animator.SetTrigger("Run");
    }

    private void UpdateChase()
    {
        timer += Time.deltaTime;

        if (distanceToTarget < pushRange)
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

        timer = 0f;
        var lookPos = target.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        animator.SetBool("Push", true);

        State = States.Run;
    }

    public override void OnPush(float strength, Vector3 hitPoint, Vector3 hitNormal)
    {

        base.OnPush(strength, hitPoint, hitNormal);

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

        animator.SetTrigger("Dei");

        var colliders = GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void UpdateDie()
    {

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
}
