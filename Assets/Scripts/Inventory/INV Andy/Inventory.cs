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
    public Transform hotbarContainer; // NUEVO: Acá vamos a arrastrar la Hotbar

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
        var itemData = db.dataBase[id];

        // 1. Primero intentamos APILAR (Stackear) si el ítem es acumulable
        if (itemData.acumulable)
        {
            // Busca lugar en Hotbar
            if (TryStackItem(hotbarContainer, id, ref quantity, itemData.maxStack)) return;
            // Busca lugar en Inventario Principal
            if (TryStackItem(slotsContainer, id, ref quantity, itemData.maxStack)) return;
        }

        // 2. Si no es acumulable o sobró cantidad, buscamos un SLOT VACÍO
        // Busca vacío en Hotbar
        if (TrySpawnInEmptySlot(hotbarContainer, id, quantity)) return;
        // Busca vacío en Inventario Principal
        if (TrySpawnInEmptySlot(slotsContainer, id, quantity)) return;

        // 3. Si llega acá, es porque no hay lugar en ningún lado
        Debug.LogWarning("¡Inventario y Hotbar llenos! No se pudo agarrar el ítem.");
    }

    // --- FUNCIONES AYUDANTES (Hacen que el código de arriba sea más limpio) ---

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
                        return true; // Se acomodó todo el stack
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

    public void SpawnItemForSave(int id, int quantity, Transform targetSlot)
    {
        // Reutilizamos la lógica con el SetParent en false
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