using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount = 27;

    void Awake() // Usamos Awake para que los slots existan antes que nada
    {
        for(int i = 0; i < slotCount; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, inventoryPanel.transform);
            newSlot.name = "Slot_" + i;
        }
    }
}
