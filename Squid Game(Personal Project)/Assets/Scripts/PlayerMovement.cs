using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 20f;

    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private Animator playerAnimator;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        playerRigidbody.velocity = new Vector3(playerInput.moveH, 0, playerInput.moveV) *moveSpeed ;

        playerAnimator.SetFloat("MoveH", playerRigidbody.velocity.x);
        playerAnimator.SetFloat("MoveV", playerRigidbody.velocity.z);
    }
    
}
