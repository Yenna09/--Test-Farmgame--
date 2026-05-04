using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject panelInventario;
    public bool inventarioAbierto; // Se sincroniza desde Inventory.cs
    public static PlayerInventory instance;

    void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    
    // Quitamos el panelInventario.SetActive del Start de aquí 
    // porque ya lo debería manejar el Inventory.cs o el InventoryController.

    void Update()
    {
        // El jugador solo da la orden, el Manager (Inventory) ejecuta
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            if (Inventory.Instance != null)
            {
                Inventory.Instance.ToogleInventory();
            }
        }
    }
}
