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
        if (Input.GetMouseButtonDown(0) && !estaUsandoHerramienta)
        {
            // --- NUEVO: PRIORIDAD 1 -> COSECHAR ---
            if (FarmingController.Instance != null && FarmingController.Instance.PuedeCosechar())
            {
                StartCoroutine(CosecharRoutine());
                return; // Cortamos acá la ejecución para que no intente usar una herramienta
            }

            // --- PRIORIDAD 2 -> USAR HERRAMIENTAS ---
            int idEnMano = HotbarController.Instance.GetEquippedItemID();

            if (idEnMano >= 0 && idEnMano < Inventory.Instance.db.dataBase.Length)
            {
                Database.InventoryItem itemEquipado = Inventory.Instance.db.dataBase[idEnMano];

                if (itemEquipado.accion == Database.ActionType.arar || 
                    itemEquipado.accion == Database.ActionType.regar || 
                    itemEquipado.accion == Database.ActionType.plantar)
                {
                    StartCoroutine(AccionHerramientaRoutine(itemEquipado.accion));
                }
            }
        }
    }

    IEnumerator AccionHerramientaRoutine(Database.ActionType accion)
    {
        estaUsandoHerramienta = true;
        rb.linearVelocity = Vector3.zero;

        // Elegimos la animación según la acción
        if (accion == Database.ActionType.arar) 
        {
            animator.SetTrigger("doSwing");
        }
        else if (accion == Database.ActionType.regar) 
        {
            animator.SetTrigger("doWater");
        }
        else if (accion == Database.ActionType.plantar) 
        {
            // Podés usar doSwing temporalmente hasta que tengas una animación de plantar
            animator.SetTrigger("doSwing"); 
        }

        // Esperamos a la mitad de la animación
        yield return new WaitForSeconds(0.3f);

        // ¡Acá es donde finalmente se llama a plantar/regar/arar!
        if (FarmingController.Instance != null)
        {
            FarmingController.Instance.UsarHerramienta();
        }

        // Terminamos de esperar
        yield return new WaitForSeconds(0.3f);
        estaUsandoHerramienta = false;
    }

    IEnumerator CosecharRoutine()
    {
        estaUsandoHerramienta = true;
        rb.linearVelocity = Vector3.zero;

        // Reproducimos la misma animación temporalmente (o podés crear una animación nueva "doPickup")
        animator.SetTrigger("doSwing"); 

        // Esperamos a la mitad de la animación
        yield return new WaitForSeconds(0.3f);

        if (FarmingController.Instance != null)
        {
            FarmingController.Instance.EjecutarCosecha();
        }

        // Terminamos de esperar
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