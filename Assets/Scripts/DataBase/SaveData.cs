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

[System.Serializable]
public class DatosCultivoGuardado
{
    public Vector3Int posicion;
    public int idSemilla;
    public int diasCrecimiento;
}
[System.Serializable]
public class DatosTerrenoGuardado
{
    public Vector3Int posicion;
    public string estado; // Guardaremos "arado" o "mojado"
}

// Definimos la estructura del inventario guardado
[System.Serializable]
public class InventorySaveData
{
    public List<SavedSlot> savedItems = new List<SavedSlot>();
}

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public InventorySaveData inventorySaveData;
    public List<string> destroyedItemsIDs;
    public int horaGuardada; 
    public int minutoGuardado; 
    public List<DatosCultivoGuardado> cultivosGuardados;
    public List<DatosTerrenoGuardado> terrenoGuardado;
}
