using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System;

public class Inventory : MonoBehaviour
{
    //Evento para checkear inventario
    public static event Action pickUpItem;
    public static Inventory Instance { get; private set; }
    public Database db;
    public Transform itemPrefab; // El prefab UI de la manzana/item
    public Transform slotsContainer; // El panel donde el Controller creo los slots
    public Transform hotbarContainer; //Acá vamos a arrastrar la Hotbar

    public GameObject inventoryToggle;
    public bool isOpen; // Esta es la variable global
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
        inventoryToggle.SetActive(false);
    }

    
    public bool PickUpItem(int id, int quantity)
    {
        //Aviso a demas scripts que se agarró un item
        
        var itemData = db.dataBase[id];

        // Primero Intenta Stackear
        if (itemData.acumulable)
        {
            if (TryStackItem(hotbarContainer, id, ref quantity, itemData.maxStack)) { pickUpItem.Invoke(); return true; } // ¡Éxito!
            if (TryStackItem(slotsContainer, id, ref quantity, itemData.maxStack)) { pickUpItem.Invoke(); return true; } // ¡Éxito!
        }

        // Buscamos un SLOT VACIO
        if (TrySpawnInEmptySlot(hotbarContainer, id, quantity)) { pickUpItem.Invoke(); return true; } // ¡Éxito!
        if (TrySpawnInEmptySlot(slotsContainer, id, quantity)) { pickUpItem.Invoke(); return true; } // ¡Éxito!

        

        // 3. Si llega aca, no hay lugar
        Debug.LogWarning("¡Inventario y Hotbar llenos! No se pudo agarrar el ítem.");
        return false; // Fallo, no hay espacio
    }

    private bool TryStackItem(Transform container, int id, ref int quantity, int maxStack)
    {
        foreach (Transform slot in container)
        {
            if (slot.childCount > 0)
            {
                ItemUI itemInSlot = slot.GetChild(0).GetComponent<ItemUI>();
                if (itemInSlot != null && itemInSlot.id == id && itemInSlot.quantity < maxStack)
                {
                    int spaceLeft = maxStack - itemInSlot.quantity;
                    if (quantity <= spaceLeft)
                    {
                        itemInSlot.quantity += quantity;
                        itemInSlot.RefreshUI();
                        return true; // Se acomodo todo el stack
                    }
                    else
                    {
                        itemInSlot.quantity = maxStack;
                        itemInSlot.RefreshUI();
                        quantity -= spaceLeft; // Acomodamos lo que entró, pero sobra cantidad
                    }
                }
            }
        }
        return false;
    }

    private bool TrySpawnInEmptySlot(Transform container, int id, int quantity)
    {
        foreach (Transform slot in container)
        {
            if (slot.childCount == 0) // Si el slot está vacío
            {
                SpawnItemInSlot(id, quantity, slot);
                return true;
            }
        }
        return false;
    }

    private void SpawnItemInSlot(int id, int quantity, Transform targetSlot)
    {
        //Instanciamos el objeto
        GameObject newItemGO = Instantiate(itemPrefab.gameObject);
        
        // El 'false' le prohíbe a Unity aplicar ese offset de -1920.
        newItemGO.transform.SetParent(targetSlot, false);

        newItemGO.name = "Item_UI_ID_" + id;

        // Inicializamos datos
        ItemUI itemUI = newItemGO.GetComponent<ItemUI>();
        if (itemUI != null)
        {
            itemUI.InitializeItem(id, quantity);
        }

        //Forzamos posición 0 por seguridad
        RectTransform rect = newItemGO.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero; // Ahora sí funcionará
            rect.localPosition = Vector3.zero;
        }
    }
    

    public void ToogleInventory()
    {
        isOpen = !isOpen;
        inventoryToggle.SetActive(isOpen);

        // Actualizamos la variable en el PlayerInventory para que el movimiento se entere
        if (PlayerInventory.instance != null)
        {
            PlayerInventory.instance.inventarioAbierto = isOpen;
        }
    }

    public void SpawnItemForSave(int id, int quantity, Transform targetSlot)
    {
        // Reutilizamos la logica con el SetParent en false
        GameObject newItemGO = Instantiate(itemPrefab.gameObject);
        newItemGO.transform.SetParent(targetSlot, false);
        newItemGO.name = "Item_UI_ID_" + id;

        ItemUI itemUI = newItemGO.GetComponent<ItemUI>();
        if (itemUI != null)
        {
            itemUI.InitializeItem(id, quantity);
        }

        // Forzamos el centrado
        RectTransform rect = newItemGO.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}