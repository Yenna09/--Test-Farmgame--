using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public int itemID; // El ID de la Database
    public int cantidad;
    
    // Referencia al SpriteRenderer para cambiar la imagen
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

    public virtual void Use() 
    { 
        Debug.Log("Usando item ID: " + itemID);
    }
}
