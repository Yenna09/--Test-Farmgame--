using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject panelInventario;
    public bool inventarioAbierto;
    public static PlayerInventory instance;

    public bool tieneAzadaEquipada = false;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {

        // Forzamos que empiece cerrado
        inventarioAbierto = false;
        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
        }
    }

    // Agregá esta función para que el botón pueda "hablarle" al código
    public void EquiparAzada()
    {
        tieneAzadaEquipada = true;
        Debug.Log("Azada equipada!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // En lugar de manejarlo él mismo, llama al Singleton del inventario grande
            if (Inventory.Instance != null)
            {
                Inventory.Instance.ToogleInventory();
            }
        }
    }

}
