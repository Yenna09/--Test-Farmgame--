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
    public int slotsCount = 10;
    public bool isOpen;
    

    [SerializeField] 
    private GameObject inventoryToggle;
    
    [SerializeField] 
    private Transform slotPrefab;

    [SerializeField] 
    private Transform itemPrefab;

    [SerializeField] 
    private PlayerInventory player;

    public DeletionPrompt deletionPrompt;
    public DescriptionUI descriptionUI;

    public List<ItemUI> items = new List<ItemUI>();
    bool itemsDeleteModeEnabled;

    [SerializeField] 
    private Transform slotsContainer;
    private List<Transform> slots = new List<Transform>();

    [SerializeField] private RectTransform selectorVisual; // Arrastra aquí la imagen del marco/brillo
    public int slotSeleccionadoIndex = 0; // Para saber cuál es el actual


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
    }
    
    void Start()
    {
        for (int i = 0; i < slotsCount; i++ )
        {
            Transform newSlot = Instantiate(slotPrefab, slotsContainer);
            slots.Add(newSlot);
        }

        isOpen = true;
        ToogleInventory();
    }

    public void UpdateParent(ItemUI item, Transform newParent)
    {
        item.exParent = newParent;
        item.transform.SetParent(newParent);
        item.transform.parent.GetComponent<Image>().fillCenter = true;
        item.transform.localPosition = Vector3.zero;
        item.EnableDeletion(itemsDeleteModeEnabled);
    }

    void Update()
    {
        // Solo permitimos cambiar de slot si el inventario está abierto
        // Opcional: si quieres que el "Hotbar" funcione siempre, quita el 'if'
        if (isOpen)
        {
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SeleccionarSlot(i);
                }
            }
        }

        // Movimiento suave del selector
        if (selectorVisual != null && selectorVisual.gameObject.activeSelf)
        {
            // Interpola la posición actual hacia la posición del slot seleccionado
            selectorVisual.position = Vector3.Lerp(selectorVisual.position, slots[slotSeleccionadoIndex].position, Time.deltaTime * 15f);
        }
    }

    public void SeleccionarSlot(int index)
    {
        // Validamos que el índice no sea mayor a la cantidad de slots que existen
        if (index < slots.Count)
        {
            slotSeleccionadoIndex = index;
            Debug.Log("Slot seleccionado: " + index);

            // Movemos el selector visual a la posición del slot seleccionado
            if (selectorVisual != null)
            {
                selectorVisual.gameObject.SetActive(true);
                selectorVisual.position = slots[index].position;
            }
        }
    }


    public void AddItem(int id, int quantity)
    {
        //Se usa para validar tu item de mierda y si la cantidad coincide con el item que se lo guarde ahi
        //Ejemplo
        //Apple max: 5 quantity: 3
        //id: 0 q: 8
        //Devuelve null
        ItemUI preexistentValidItem = items.Find(item => item.itemData.ID == id && item.itemData.maxStack >= item.quantity + quantity);
        if(preexistentValidItem != null)
        {
            preexistentValidItem.quantity += quantity;
            return;
        }
        for(int i = 0; i < slots.Count; i++)
        {
            ItemUI itemInSlot = slots[i].childCount == 0 ? null : slots[i].GetChild(0).GetComponent<ItemUI>();

            if(itemInSlot == null)
            {
                ItemUI itemCopy = Instantiate(itemPrefab, transform).GetComponent<ItemUI>();

                itemCopy.InitializeItem(id, quantity);
                items.Add(itemCopy);

                UpdateParent(itemCopy, slots[i]);
                break;
            }
            else if(itemInSlot.id == id && itemInSlot.itemData.maxStack >= itemInSlot.quantity + quantity)
                {
                    itemInSlot.quantity += quantity;
                    break;
                }
        }
    }

    public ItemUI GetSelectedItem()
    {
        // Revisamos si el slot actual tiene un objeto adentro
        if (slots[slotSeleccionadoIndex].childCount > 0)
        {
            return slots[slotSeleccionadoIndex].GetChild(0).GetComponent<ItemUI>();
        }
        return null; // El slot está vacío
    }

    public void DeleteItem( ItemUI item, int quantity, bool byUse)
    {
        //ItemUI itemToDelete = item.Find(it => it == item);

        //itemToDelete.quantity -= quantity;

        if(!byUse)
        {
            BaseItem spawnedItem = Instantiate(item.itemData.item);
            //spawnedItem.transform.position = player.itemSpawn.position;
            //spawnedItem.SetDataById(item.id, quantity);
        }
        //if(itemToDelete.quantity <= 0)
        {
            //itemToDelete.exParent.GetComponent<Image>().fillCenter = false;
            //items.Remove(itemToDelete);
            //Destroy(itemToDelete.gameObject);
        }
    }

    public void ToggleDeleteMode()
    {
        itemsDeleteModeEnabled = !itemsDeleteModeEnabled;
        foreach(ItemUI item in items)
        {
            item.EnableDeletion(itemsDeleteModeEnabled);
        }
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

    public void ShowDescription(ItemUI item)
    {
        descriptionUI.gameObject.SetActive(true);
        //descriptionUI.Show(item);
    }

    public void HideDescription()
    {
        descriptionUI.gameObject.SetActive(false);
        Debug.Log("Ocultando descripción del inventario");
    }

    public void ShowDeletionPrompt(ItemUI item)
    {
        deletionPrompt.gameObject.SetActive(true);
        //deletionPrompt.SetSliderData(item);
    }

    public void AddMoreSpace(int slotsToAdd)
    {
        for (int i = 0; i < slotsToAdd; i++)
        {
            Transform newSlot = Instantiate(slotPrefab, slotsContainer);
            slots.Add(newSlot);
        }
    }
}