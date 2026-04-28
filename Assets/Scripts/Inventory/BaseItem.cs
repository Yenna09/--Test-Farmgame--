using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public int itemID; // El ID de la Database
    public int cantidad;
    
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ConfigurarObjeto(int id, int cant, Sprite imagen)
    {
        itemID = id;
        cantidad = cant;
        if(spriteRenderer != null) spriteRenderer.sprite = imagen;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Verifica que el objeto que entra tenga el Tag "Player"
        if (other.CompareTag("Player"))
        {
            // Llamamos al inventario para que lo guarde con prioridad en Hotbar
            Inventory.Instance.PickUpItem(itemID, cantidad);
            
            // Destruimos el objeto del suelo
            Destroy(gameObject);
        }
    }

    public virtual void Use() 
    { 
        Debug.Log("Usando item ID: " + itemID);
    }
}
