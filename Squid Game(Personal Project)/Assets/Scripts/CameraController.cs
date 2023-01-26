using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed;
    private Vector3 cameraOffset;

    public GameObject player;

    private void Start()
    {
        cameraSpeed = player.GetComponent<PlayerMovement>().moveSpeed;
        cameraOffset = transform.position;
    }

    private void Update()
    {
        Vector3 dir = player.transform.position + cameraOffset;
        this.transform.position = dir;
    }
}