using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject GO;

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
        playerNavMeshAgent.velocity = new Vector3(playerInput.moveH, 0, playerInput.moveV) *moveSpeed ;

        playerAnimator.SetFloat("MoveH", playerNavMeshAgent.velocity.x);
        playerAnimator.SetFloat("MoveV", playerNavMeshAgent.velocity.z);
    }
    
}
