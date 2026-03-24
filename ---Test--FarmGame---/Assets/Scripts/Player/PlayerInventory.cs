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
    
        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
            inventarioAbierto = false; // Nos aseguramos que empiece cerrado
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventarioAbierto = !inventarioAbierto;
            panelInventario.SetActive(inventarioAbierto);
        }
    }

}
