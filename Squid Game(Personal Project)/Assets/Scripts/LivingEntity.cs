using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class LivingEntity : MonoBehaviour, IPushable, IDamageable
{
    public enum States
    {
        None = -1,
        Idle,
        Run,
        Chase,
        Push,
        FallDown,
        Stop,
        Die,
    }

    private Rigidbody rb;
    public NavMeshAgent agent;

    public float mass;
    public float speed;
    public float chaseSpeed;
    public float power;

    public bool dead { get; protected set; }
    public event Action onDeath;

    private List<LivingEntity> targets = new List<LivingEntity>();


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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

    public virtual void OnDamage()
    {      
        Die();
    }

    public virtual void Die()
    {
        if (onDeath != null)
        {
            onDeath();
        }

        dead = true;
    }
}
