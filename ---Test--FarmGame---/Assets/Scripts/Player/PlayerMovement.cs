using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))] // Nos aseguramos que siempre haya un Animator
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 5f;
    public float verticalCompensation = 1.25f;

    private Rigidbody rb;
    private Animator animator; // 1. Referencia al Animator
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // 2. Inicializamos el Animator
        rb.freezeRotation = true;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(horizontal, 0, vertical).normalized;

        // 3. Enviamos los datos al Animator
        // "Horizontal" y "Vertical" le dicen al Blend Tree hacia dónde mirar.
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);

        // "Speed" sirve para saber si el personaje se está moviendo (mayor a 0) o está quieto (0).
        // Usamos sqrMagnitude porque es más eficiente que calcular la distancia exacta.
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        // Opcional: Esto guarda la última dirección para que al dejar de moverte
        // el personaje se quede mirando hacia donde caminaba en lugar de resetearse.
        if (horizontal != 0 || vertical != 0)
        {
            animator.SetFloat("LastHorizontal", horizontal);
            animator.SetFloat("LastVertical", vertical);
        }
    }

    void FixedUpdate()
    {
        // Tu lógica de movimiento física se mantiene igual
        Vector3 targetVelocity = moveInput * speed;
        targetVelocity.z *= verticalCompensation; // Compensación de perspectiva 2.5D

        targetVelocity.y = -5f; // Gravedad manual para mantenerlo al suelo

        rb.velocity = targetVelocity;
    }
}