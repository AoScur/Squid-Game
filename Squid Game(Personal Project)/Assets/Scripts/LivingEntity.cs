using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
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
        OnPush,
        Die,
    }
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public float mass, speed, chaseSpeed, power;

    public GameObject rightBall;

    public bool dead { get; protected set; }
    public event Action onDeath;

    public States state = States.None;
    public virtual States State { get; set; }

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator= GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        dead = false;        
    }

    public virtual void OnPush(Vector3 hitPoint, Vector3 hitNormal)
    {
        Vector3 force = hitNormal * 5f;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.velocity = Vector3.zero;
        rb.AddForce(force,ForceMode.Impulse);

        var hitEffect = GameManager.hit_Pool.Get();
        hitEffect.transform.position = hitPoint;
        hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
        hitEffect.GetComponent<ParticleSystem>().Play();
        hitEffect.Hit();
    }

    public virtual void OnDie(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (onDeath != null)
        {
            onDeath();
        }

        dead = true;

        var bloodSprayEffect = GameManager.blood_Pool.Get();
        bloodSprayEffect.transform.position = hitPoint;
        bloodSprayEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
        bloodSprayEffect.GetComponent<ParticleSystem>().Play();
        bloodSprayEffect.Bleeding();
    }
}
