using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class Inventory : MonoBehaviour
{
    public GraphicRaycaster graphRay;
    public Database db; // Asegúrate que tu script de base de datos se llame "Database"
    public int slotsCount = 35;
    public bool isOpen;

    [SerializeField] 
    private GameObject inventoryToggle;
    [SerializeField] 
    private Transform slotPrefab;
    [SerializeField] 
    private Transform itemPrefab;
    [SerializeField] 
    //private Player player;

    public List<ItemUI> items = new List<ItemUI>();
    bool itemsDeleteModeEnabled;

    [SerializeField] private Transform slotsContainer;
    private List<Transform> slots = new List<Transform>();

    public static Inventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
    }

    public void UpdateParent(ItemUI item, Transform newParent)
    {
        item.exParent = newParent;
        item.transform.SetParent(newParent);
        var img = item.transform.parent.GetComponent<Image>();
        if (img != null) img.fillCenter = true;
        item.transform.localPosition = Vector3.zero;
        item.EnableDeletion(itemsDeleteModeEnabled);
    }

    // ESTA ES LA FUNCIÓN QUE TE FALTABA Y CAUSABA EL ERROR ROJO
    public void DeleteItem(ItemUI item, int amount, bool destroyObject)
    {
        item.quantity -= amount;
        if (item.quantity <= 0 || destroyObject)
        {
            items.Remove(item);
            Destroy(item.gameObject);
        }
    }
}