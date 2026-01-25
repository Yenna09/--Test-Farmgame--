using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    
    
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
        
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}
