using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public GraphicRaycaster graphRay;
    public Database db; // Asegúrate que tu script de base de datos se llame "Database"
    //public int slotsCount = 27;
    public bool isOpen;
    

    [SerializeField] 
    private GameObject inventoryToggle;

    [SerializeField] 
    private Transform itemPrefab;

    [SerializeField] 
    private PlayerInventory player;
    bool itemsDeleteModeEnabled;

    [SerializeField] 
    private Transform slotsContainer;
    private List<Transform> slots = new List<Transform>();

    


    private void Awake()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Debug.Log("Slot índice " + i + " se llama: " + slots[i].name);
        }
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
    }
    
    void Start()
    {
        // Buscamos todos los slots que pusimos manualmente en el Container
        foreach (Transform child in slotsContainer)
        {
            if (child.GetComponent<InventorySlot>())
            {
                slots.Add(child);
            }
        }

        isOpen = true;
        ToogleInventory(); //
    }

    public void UpdateParent(ItemUI item, Transform newParent)
    {
        item.exParent = newParent;
        item.transform.SetParent(newParent);
        item.transform.parent.GetComponent<Image>().fillCenter = true;
        item.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
                
    }




    public void ToogleInventory()
    {
        isOpen = !isOpen;
        inventoryToggle.SetActive(isOpen);

        // NUEVO: Le avisamos al script del conejo que el inventario cambió
        if (player != null)
        {
            player.inventarioAbierto = isOpen;
        }
    }


    public void SpawnItemInSlot(int id, int quantity, Transform targetSlot)
    {
        // Instanciamos el prefab
        GameObject newItemGO = Instantiate(itemPrefab.gameObject);
        newItemGO.name = itemPrefab.name; // Esto evita el "(Clone)" y ayuda a la consistencia
        
        // IMPORTANTE: Primero configuramos los datos
        ItemUI newItemUI = newItemGO.GetComponent<ItemUI>();
        newItemUI.InitializeItem(id, quantity);

        // Obtenemos el RectTransform
        RectTransform rect = newItemGO.GetComponent<RectTransform>();

        // 1. Lo emparentamos con 'false' para que no intente mantener su escala del mundo
        rect.SetParent(targetSlot, false);

        // 2. Reseteamos escala y posición
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;

        // 3. FORZAMOS EL STRETCH (Esto es lo que evita que se vea chico)
        // Ponemos los anclajes de esquina a esquina (0 a 1)
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        
        // Ponemos los offsets en 0 para que se pegue a los bordes del slot
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    // Fragmento corregido de Inventory.cs
    public void PickUpItem(int id, int quantity)
    {
        // 1. BUSCAR STACK EXISTENTE: Verificamos si ya tenemos el item y si es acumulable
        if (db.dataBase[id].acumulable) 
        {
            foreach (Transform slot in slots)
            {
                if (slot.childCount > 0)
                {
                    ItemUI itemInSlot = slot.GetChild(0).GetComponent<ItemUI>();
                    // Si el ID coincide y no hemos superado el máximo stack
                    if (itemInSlot.id == id && itemInSlot.quantity < db.dataBase[id].maxStack)
                    {
                        itemInSlot.quantity += quantity;
                        itemInSlot.RefreshUI();
                        return; // Terminamos: se sumó al stack existente
                    }
                }
            }
        }

        // 2. SI NO HAY STACK: Buscar primer slot vacío (Prioridad Hotbar 0-8)
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].childCount == 0)
            {
                SpawnItemInSlot(id, quantity, slots[i]);
                return;
            }
        }
    }

}