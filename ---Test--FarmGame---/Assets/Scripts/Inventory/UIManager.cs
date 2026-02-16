using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Transform hotbarContainer;   // Asigna el HotbarPanel
    public Transform backpackContainer; // Asigna el BackpackPanel

    public GameObject slotPrefab;

    private void Start()
    {
        Instance = this;
        InitializeSlots();
    }

    void InitializeSlots()
    {
        // Crear 9 slots visuales en la Hotbar
        for (int i = 0; i < 9; i++)
        {
            Instantiate(slotPrefab, hotbarContainer);
        }
        // Crear 18 slots visuales en la Mochila
        for (int i = 0; i < 18; i++)
        {
            Instantiate(slotPrefab, backpackContainer);
        }
    }

    public void UpdateUI()
    {
        var slotsData = InventoryManager.Instance.inventorySlots;

        // Actualizar Hotbar (Indices 0-8)
        for (int i = 0; i < 9; i++)
        {
            UpdateSlotVisual(hotbarContainer.GetChild(i), slotsData[i]);
        }

        // Actualizar Mochila (Indices 9-26)
        // OJO: El índice de la mochila visual empieza en 0, pero en la data empieza en 9
        for (int i = 0; i < 18; i++)
        {
            UpdateSlotVisual(backpackContainer.GetChild(i), slotsData[i + 9]);
        }
    }

    void UpdateSlotVisual(Transform slotObj, InventorySlot data)
    {
        Image icon = slotObj.Find("Icon").GetComponent<Image>();
        // Lógica para activar/desactivar icono según si data.IsEmpty
    }
}