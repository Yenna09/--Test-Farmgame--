using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject panelInventario;
    public bool inventarioAbierto;
    public static PlayerInventory instance;

    void Awake() 
    {
        instance = this;
    }
    
    void Start()
    {
    
        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
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
