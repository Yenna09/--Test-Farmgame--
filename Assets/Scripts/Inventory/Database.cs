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
        public Tipo tipo;
        public bool acumulable;
        public int maxStack;
        public string descripcion;
        public BaseItem item;
    }

    public enum Tipo
    {
        consumible,
        equipable
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
