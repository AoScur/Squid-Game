using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private NavMeshAgent playerNavMeshAgent;
    private NavMeshObstacle playerNavMeshObstacle;
    private Animator playerAnimator;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        playerNavMeshAgent = GetComponent<NavMeshAgent>();
        playerNavMeshObstacle = GetComponent<NavMeshObstacle>();
    }

    private void FixedUpdate()
    {
        playerRigidbody.velocity = new Vector3(playerInput.moveH, 0, playerInput.moveV) *moveSpeed ;

        playerAnimator.SetFloat("MoveH", playerRigidbody.velocity.x);
        playerAnimator.SetFloat("MoveV", playerRigidbody.velocity.z);
    }
    
}
