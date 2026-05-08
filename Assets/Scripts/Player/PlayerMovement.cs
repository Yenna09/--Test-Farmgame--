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

    [Header("Configuración de Herramientas")]
    public int idDeLaAzada = 7; 
    public int idDeLaRegadera = 13;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveInput;

    private bool estaUsandoHerramienta = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Centralizamos el chequeo del inventario acá
        bool inventarioAbierto = false;
        if (Inventory.Instance != null) inventarioAbierto = Inventory.Instance.isOpen;

        if (estaUsandoHerramienta || inventarioAbierto)
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

        // --- LA MAGIA ACÁ ---
        // Si hace clic izquierdo, preguntamos a la Hotbar si tiene la Azada
        if (Input.GetMouseButtonDown(0))
        {
            int itemEquipado = HotbarController.Instance.GetEquippedItemID();
            
            if (itemEquipado == idDeLaAzada || itemEquipado == idDeLaRegadera)
            {
                StartCoroutine(AccionHerramientaRoutine(itemEquipado));
            }
        }
    }

    IEnumerator AccionHerramientaRoutine(int itemID)
    {
        estaUsandoHerramienta = true;
        rb.linearVelocity = Vector3.zero;

        // Disparamos el trigger según qué tengamos en la mano
        if (itemID == idDeLaAzada) animator.SetTrigger("doSwing");
        else if (itemID == idDeLaRegadera) animator.SetTrigger("doWater"); // Asegurate de tener este trigger

        yield return new WaitForSeconds(0.3f);

        if (FarmingController.Instance != null)
        {
            FarmingController.Instance.UsarHerramienta();
        }

        yield return new WaitForSeconds(0.3f);
        estaUsandoHerramienta = false;
    }
    void FixedUpdate()
    {    
        bool inventarioAbierto = false;
        if (Inventory.Instance != null) inventarioAbierto = Inventory.Instance.isOpen;

        if (inventarioAbierto || estaUsandoHerramienta)
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
            targetVelocity.y = -5f; 
            rb.linearVelocity = targetVelocity;
        }
    }
}