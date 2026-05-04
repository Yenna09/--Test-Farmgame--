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

    // NUEVO: Variable para saber si est� animando
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

        // L�gica del click
        if (Input.GetMouseButtonDown(0) && PlayerInventory.instance.tieneAzadaEquipada)
        {
            Vector3Int position = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z );
            // Disparamos la corrutina para bloquear el movimiento
            StartCoroutine(UsarHerramientaRoutine());
        }
    }

    // NUEVO: Corrutina que gestiona el tiempo de la acci�n
    IEnumerator UsarHerramientaRoutine()
    {
        estaUsandoHerramienta = true;
        rb.linearVelocity = Vector3.zero;

        // ASEGURATE DE ESTO: 
        // Forzamos al Animator a usar la �ltima direcci�n guardada antes del swing
        animator.SetFloat("LastHorizontal", animator.GetFloat("LastHorizontal"));
        animator.SetFloat("LastVertical", animator.GetFloat("LastVertical"));

        animator.SetTrigger("doSwing");

        // Esperamos el tiempo del clip (ej: 0.6 segundos)
        yield return new WaitForSeconds(0.6f);

        estaUsandoHerramienta = false;
    }

    void FixedUpdate()
    {    
        // Usamos el estado centralizado del Inventario
        if (Inventory.Instance.isOpen || estaUsandoHerramienta)
        {
            rb.linearVelocity = Vector3.zero;
            // Ojo: FreezeAll puede dar tirones si se hace cada frame, 
            // pero para un inventario estático está bien.
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