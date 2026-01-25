using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float verticalCompensation = 1.25f;
    
    
    private Rigidbody rb;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(horizontal, 0, vertical).normalized;
    }

    void FixedUpdate()
    {
        Vector3 targetVelocity = moveInput * speed;

        targetVelocity.z *= verticalCompensation;

        targetVelocity.y = rb.velocity.y;

        rb.velocity = targetVelocity;
    }
}
