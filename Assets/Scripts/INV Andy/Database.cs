using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DataBase", menuName = "Inventory/New Database", order = 1)]
public class Database : ScriptableObject
{
    [System.Serializable]
    public struct InventoryItem
    {
        public string nombre;
        public int ID;
        public Sprite icono;
        public ItemType tipo;
        public ActionType accion;
        public bool acumulable;
        public int maxStack;
        public string descripcion;
        public BaseItem item;
    }

    public enum ItemType
    {
        consumible,
        equipable, 
        mision,
    }

    public enum ActionType
    {
        arar,
        consumir,
        plantar,
        picar,
        regar,
        talar,
        
        
    }

    public InventoryItem[] dataBase;

    // se llama cuando se carga el scriptable object y cuando este cambia en el Inspector
    private void OnValidate()
    {
        if (dataBase != null)
        {
            for (int i = 0; i < dataBase.Length; i++)
            {
                if (dataBase[i].ID != i)
                {
                    dataBase[i].ID = i;
                }
            }
        }
    }
}
