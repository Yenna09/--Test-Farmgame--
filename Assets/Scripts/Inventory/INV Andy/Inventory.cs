using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public Database db;
    public Transform itemPrefab; // El prefab UI de la manzana/item
    public Transform slotsContainer; // El panel donde el Controller creó los slots

    public GameObject inventoryToggle;
    public bool isOpen; // Esta es la variable global
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
        inventoryToggle.SetActive(false);
    }

   public void PickUpItem(int id, int quantity)
    {
        // --- CHEQUEOS DE SEGURIDAD ---
        if (db == null) {
            Debug.LogError("ERROR: No asignaste la Database en el objeto Inventory del Inspector.");
            return;
        }
        
        if (slotsContainer == null) {
            Debug.LogError("ERROR: No asignaste el slotsContainer en el Inspector.");
            return;
        }

        // 1. Lógica de STACK
        if (db.dataBase[id].acumulable)
        {
            foreach (Transform slot in slotsContainer)
            {
                if (slot.childCount > 0)
                {
                    ItemUI itemInSlot = slot.GetChild(0).GetComponent<ItemUI>();
                    if (itemInSlot != null && itemInSlot.id == id && itemInSlot.quantity < db.dataBase[id].maxStack)
                    {
                        itemInSlot.quantity += quantity;
                        itemInSlot.RefreshUI();
                        return;
                    }
                }
            }
        }

        // 2. Lógica de ESPACIO VACÍO
        foreach (Transform slot in slotsContainer)
        {
            if (slot.childCount == 0)
            {
                SpawnItemInSlot(id, quantity, slot);
                return;
            }
        }
        
        Debug.Log("Inventario lleno");
    }

    private void SpawnItemInSlot(int id, int quantity, Transform targetSlot)
    {
        // 1. Instanciamos el objeto SIN emparentarlo en el mismo paso
        GameObject newItemGO = Instantiate(itemPrefab.gameObject);
        
        // 2. EL FIX DEFINITIVO: SetParent con 'false'. 
        // El 'false' le prohíbe a Unity aplicar ese offset de -1920.
        newItemGO.transform.SetParent(targetSlot, false);

        newItemGO.name = "Item_UI_ID_" + id;

        // Inicializamos datos
        ItemUI itemUI = newItemGO.GetComponent<ItemUI>();
        if (itemUI != null)
        {
            itemUI.InitializeItem(id, quantity);
        }

        // 3. Forzamos posición 0 por seguridad
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
}