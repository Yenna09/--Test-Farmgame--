using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 5f;
    public float verticalCompensation = 1.25f; 

    private Rigidbody rb;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(horizontal, 0, vertical).normalized;
    }

    void FixedUpdate()
    {
        // 1. Calculamos la velocidad deseada en X y Z
        Vector3 targetVelocity = moveInput * speed;
        targetVelocity.z *= verticalCompensation;

        // En lugar de "rb.velocity.y" (que conserva saltos y rebotes),
        // ponemos un valor negativo fijo (ej. -2 o -5).
        // Esto actúa como un imán que lo mantiene pegado al suelo constantemente.
        targetVelocity.y = -5f;

        rb.velocity = targetVelocity;
    }
}