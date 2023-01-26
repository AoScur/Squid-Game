using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;


public class AIPlayer: LivingEntity
{
    public  enum AIType
    {
        None = -1,
        Heavy,
        Speedy,
        Strongy,
    }



    private List<GameObject> Alives = new List<GameObject>();
    private Transform target;
    public Transform goalLine;

    private Animator animator;

    private float distanceToGoalLine;
    private float distanceToTarget;


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
            runDestination.z = goalLine.transform.position.z;

            if (prevState == state)
                return;

            switch (state)
            {
                case States.Idle:
                    agent.isStopped = true;
                    break;
                case States.Run:
                    agent.speed = speed;
                    agent.isStopped = false;
                    agent.SetDestination(runDestination);
                    break;
                case States.Chase:
                    agent.speed = chaseSpeed;
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    break;
                case States.Push:
                    agent.isStopped = true;
                    break;
                case States.Stop:
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
 
    }

    private void Start()
    {
        State = States.Idle;

    }

    private void Update()
    {
        if (State != States.Die)
        {
            chase();
        }
    }

    public void Setup(AIData data)
    {
        speed = data.speed;
        mass = data.mass;
        power = data.power;
    }

    private void behavior()
    {
        switch(type)
        {
            case AIType.Heavy:
            {
              
              break;
            }
            case AIType.Speedy:
            {

              break;
            }
            case AIType.Strongy:
            {

              break;
            }
        }
    }

    private void chase()
    {
        target = ;
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        var wayPoint = waypoints[index].position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(wayPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            wayPoint = hit.position;
        }
        distanceToWayPoint = Vector3.Distance(transform.position, wayPoint);
    }
}
