using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Item", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string nombre;
    public Sprite icono;
    public bool esAcumulable; // Para stacks (ej. 99 semillas)
    public int maxStack = 99;
}

[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int cantidad;

    public InventorySlot(ItemData item, int cant)
    {
        itemData = item;
        cantidad = cant;
    }

    public void AddAmount(int value) => cantidad += value;
    public bool IsEmpty => itemData == null;

    public void Clear()
    {
        itemData = null;
        cantidad = 0;
    }
}