using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Player : LivingEntity
{
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;

    private Animator animator;
    private float distanceToTarget;

    public float coolDown = 0.25f;

    private States state = States.None;

    public States State
    {
        get { return state; }
        private set
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

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        speed = 5f;
    }

    private void Start()
    {
        GameManager.targets.Add(this);
        State = States.Idle;
    }

    private void Update()
    {
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
        if (State == States.Push || State == States.OnPush)
            return;

        var input = new Vector3(playerInput.moveH, 0, playerInput.moveV);
        if (input.Equals(Vector3.zero))
        {
            State = States.Run;
        }
        else
        {
            State = States.Idle;
        }

        playerRigidbody.MovePosition(transform.position + input * Time.deltaTime * speed);
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
        //var hitPoint = target.GetComponent<CapsuleCollider>().ClosestPoint(transform.position);
        //var hitNormal = (transform.position - target.transform.position).normalized;

        //target.OnPush(hitPoint, hitNormal);
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

        //hurtEffect.transform.position = hitPoint;
        //hurtEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
        //hurtEffect.Play();

        //zomAudioPlayer.PlayOneShot(hurtClip);
        base.OnDie(hitPoint, hitNormal);

        animator.SetTrigger("Die");
    }
}

