using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Player : LivingEntity
{
    public LayerMask whatIsTarget;
    private PlayerInput playerInput;

    
    private float distanceToTarget;

    public float coolDown = 0.25f;
        

    public override States State
    {
        get { return state; }
        set
        {
            var prevState = state;
            state = value;

            if (prevState == state)
                return;


            switch (state)
            {
                case States.Idle:
                    break;
                case States.Run:
                    break;
                case States.Push:
                    animator.SetBool("Push", true);
                    break;
                case States.OnPush:
                    break;
                case States.Die:
                    break;
            }
        }
    }

    public override void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        //playerRigidbody = GetComponent<Rigidbody>();
        //rb = GetComponent<Rigidbody>();

        base.Awake();

        speed = 5f;
    }

    private void Start()
    {
        GameManager.targets.Add(this);
        this.onDeath += () => GameManager.targets.Remove(this);
        State = States.Idle;
    }

    private void Update()
    {
        if (dead)
        {
            return;
        }

        if (playerInput.push)
        {
            State = States.Push;
        }
        switch (State)
        {
            case States.Push:
                UpdatePush();
                break;
            case States.OnPush:
                UpdateOnPush();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (dead)
        {
            return;
        }

        if (State == States.Push || State == States.OnPush)
            return;

        var input = new Vector3(playerInput.moveH, 0, playerInput.moveV);
        if (input.Equals(Vector3.zero))
        {
            State = States.Idle;
        }
        else
        {
            State = States.Run;
        }

        rb.MovePosition(transform.position + input * Time.deltaTime * speed);
        animator.SetFloat("MoveH", playerInput.moveH);
        animator.SetFloat("MoveV", playerInput.moveV);
    }

    private void UpdatePush()
    {
        if (animator.GetBool("Push"))
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Push") && info.normalizedTime >= 1f)
            {
                animator.SetBool("Push", false);
                State = States.Idle;
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
                State = States.Idle;
            }
        }
    }

    public void Kick()
    {
        var colliders = Physics.OverlapSphere(rightBall.transform.position, 0.3f, whatIsTarget);

        foreach(var collider in colliders)
        {
            var hitPoint = rightBall.GetComponent<CapsuleCollider>().ClosestPoint(transform.position);
            var hitNormal = (transform.position - collider.transform.position).normalized;

            collider.gameObject.GetComponent<LivingEntity>().OnPush(hitPoint, hitNormal);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rightBall.transform.position, 0.3f);
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
        animator.SetTrigger("Die");
        base.OnDie(hitPoint, hitNormal);
    }
}

