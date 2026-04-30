using System.Collections.Generic;
using UnityEngine;

// Definimos qué datos se guardan de cada slot individual
[System.Serializable]
public class SavedSlot
{
    public int slotIndex; // Posición del slot (0, 1, 2...)
    public int itemID;    // ID del ítem en la base de datos
    public int quantity;  // Cantidad acumulada
    public bool isHotbar; // true = Hotbar, false = Inventario principal
}

// Definimos la estructura del inventario guardado
[System.Serializable]
public class InventorySaveData
{
    public List<SavedSlot> savedItems = new List<SavedSlot>();
}

// Tu clase principal queda igual, pero ahora sabe qué es InventorySaveData
[System.Serializable]
public class SaveData
{
   public Vector3 playerPosition;
   public InventorySaveData inventorySaveData;
   
   // NUEVO: La lista negra de ítems que ya agarramos del piso
   public List<string> destroyedItemsIDs = new List<string>(); 
}
