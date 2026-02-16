using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // Configuración
    public int hotbarSize = 9;
    public int backpackSize = 18;
    public List<InventorySlot> inventorySlots; // Lista total de 27

    private void Awake()
    {
        Instance = this;
        // Inicializamos la lista vacía con 27 slots
        inventorySlots = new List<InventorySlot>(hotbarSize + backpackSize);
        for (int i = 0; i < hotbarSize + backpackSize; i++)
        {
            inventorySlots.Add(new InventorySlot(null, 0));
        }
    }

    public bool AddItem(ItemData item, int amount)
    {
        // 1. Buscar si ya existe para stackear (si es acumulable)
        // ... Lógica de stack ...

        // 2. Buscar primer espacio vacío
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].IsEmpty)
            {
                inventorySlots[i].itemData = item;
                inventorySlots[i].cantidad = amount;
                // ¡IMPORTANTE!: Aquí llamarías a un evento para actualizar la UI
                UIManager.Instance.UpdateUI();
                return true;
            }
        }
        return false; // Inventario lleno
    }
}