using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float speed = 5f;
    public float verticalCompensation = 1.25f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveInput;

    // NUEVO: Variable para saber si está animando
    private bool estaUsandoHerramienta = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Si estamos usando la herramienta, no leemos el input de movimiento
        if (estaUsandoHerramienta || PlayerInventory.instance.inventarioAbierto)
        {
            moveInput = Vector3.zero;
            return;
        }

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

        // Lógica del click
        if (Input.GetMouseButtonDown(0) && PlayerInventory.instance.tieneAzadaEquipada)
        {
            // Disparamos la corrutina para bloquear el movimiento
            StartCoroutine(UsarHerramientaRoutine());
        }
    }

    // NUEVO: Corrutina que gestiona el tiempo de la acción
    IEnumerator UsarHerramientaRoutine()
    {
        estaUsandoHerramienta = true;

        // Frenamos el Rigidbody de golpe
        rb.linearVelocity = Vector3.zero;

        // Disparamos la animación
        animator.SetTrigger("doSwing");

        // Esperamos a que la animación termine (ajustá 0.5f según dure tu clip)
        yield return new WaitForSeconds(0.5f);

        estaUsandoHerramienta = false;
    }

    void FixedUpdate()
    {
        // Bloqueo total si el inventario está abierto o está trabajando
        if (PlayerInventory.instance.inventarioAbierto || estaUsandoHerramienta)
        {
            rb.linearVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            Vector3 targetVelocity = moveInput * speed;
            targetVelocity.z *= verticalCompensation;
            targetVelocity.y = -5f; // Gravedad manual
            rb.linearVelocity = targetVelocity;
        }
    }
}