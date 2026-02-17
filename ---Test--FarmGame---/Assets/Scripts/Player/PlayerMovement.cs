using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))] // Nos aseguramos que siempre haya un Animator
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float speed = 5f;
    public float verticalCompensation = 1.25f;
    

    private Rigidbody rb;
    private Animator animator; 
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); 
        rb.freezeRotation = true;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(horizontal, 0, vertical).normalized;

        
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);

        
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

    
        if (horizontal != 0 || vertical != 0)
        {
            animator.SetFloat("LastHorizontal", horizontal);
            animator.SetFloat("LastVertical", vertical);
        }

    }

    void FixedUpdate()
    {
        if (InventarioToggle.instance.inventarioAbierto)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            return; 
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            Vector3 targetVelocity = moveInput * speed;
            targetVelocity.z *= verticalCompensation; 

            targetVelocity.y = -5f; 

            rb.velocity = targetVelocity;
        }
        
    }
}