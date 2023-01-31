using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class LivingEntity : MonoBehaviour, IPushable, IDieable
{
    public enum States
    {
        None = -1,
        Idle,
        Run,
        Chase,
        Push,
        FallDown,
        Die,
    }

    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public float mass, speed, chaseSpeed, power;

    public bool dead { get; protected set; }
    public event Action onDeath;

    private List<LivingEntity> targets = new List<LivingEntity>();


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        dead = false;        
    }

    public virtual void OnPush(float strength, Vector3 hitPoint, Vector3 hitNormal)
    {
        Vector3 force = (hitNormal - hitPoint) * strength;
        rb.AddForce(force);
    }

    public virtual void OnDie(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (onDeath != null)
        {
            onDeath();
        }

        dead = true;
    }
}
