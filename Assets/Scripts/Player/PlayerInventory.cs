using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    // Agreg� esta funci�n para que el bot�n pueda "hablarle" al c�digo
    public void EquiparAzada()
    {
        tieneAzadaEquipada = true;
        Debug.Log("Azada equipada!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            // En lugar de manejarlo �l mismo, llama al Singleton del inventario grande
            if (Inventory.Instance != null)
            {
                Inventory.Instance.ToogleInventory();
            }
        }
    }

}
