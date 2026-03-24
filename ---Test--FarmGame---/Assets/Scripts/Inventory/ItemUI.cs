using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Database db;

    [SerializeField]
    private GameObject deleteButton;

    public int id;
    public int quantity;

    [HideInInspector]
    public Database.InventoryItem itemData;
    [HideInInspector]
    public Transform exParent;

    TextMeshProUGUI quantityText;
    Image iconoImage;
    Vector3 dragOffset;

    void Awake()
    {
        quantityText = transform.GetComponentInChildren<TextMeshProUGUI>();
        iconoImage = GetComponent<Image>();

        exParent = transform.parent;
        if (exParent.GetComponent<Image>())
        {
            exParent.GetComponent<Image>().fillCenter = true;
        }

        InitializeItem(id, quantity);
    }

    void Update()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity.ToString();
        }
    }

    public void Delete()
    {
        Inventory.Instance.HideDescription();

        if(quantity > 1)
        {
            Inventory.Instance.ShowDeletionPrompt(this);
        }
        else
        {
            Inventory.Instance.DeleteItem(this, 1, false);
        }
    }

    public void InitializeItem(int id, int quantity)
    {
        itemData.ID = id;
        itemData.acumulable = db.dataBase[id].acumulable;
        itemData.descripcion = db.dataBase[id].descripcion;
        itemData.icono = db.dataBase[id].icono;
        itemData.nombre = db.dataBase[id].nombre;
        itemData.tipo = db.dataBase[id].tipo;
        itemData.maxStack = db.dataBase[id].maxStack;
        itemData.item = db.dataBase[id].item;

        deleteButton.SetActive(false);
        iconoImage.sprite = itemData.icono;

        this.quantity = quantity;
    }

    // Métodos solicitados agregados
    public void EnableDeletion(bool enable) { if(deleteButton != null) deleteButton.SetActive(enable); }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!eventData.dragging)
        {
            Inventory.Instance.ShowDescription(this);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Inventory.Instance.HideDescription();
            itemData.item.Use();
            Inventory.Instance.DeleteItem(this, 1, true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Inventory.Instance.HideDescription(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Inventory.Instance.HideDescription();
        quantityText.enabled = false;
        exParent = transform.parent;
        exParent.GetComponent<Image>().fillCenter = false;
        transform.SetParent(Inventory.Instance.transform);
        dragOffset = transform.position - Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        quantityText.enabled = true;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        Transform slot = null;

        // Casteo un ray desde la posicion del mouse y guardo todo lo que toca en raycastResults
        Inventory.Instance.graphRay.Raycast(eventData, raycastResults);

        // Itero todos los colliders tocados
        foreach (RaycastResult hit in raycastResults)
        {
            var hitObj = hit.gameObject;

            if (hitObj.CompareTag("Slot") && hit.gameObject.transform.childCount == 0)
            {
                slot = hit.gameObject.transform;
                break;
            }

            if (hitObj.CompareTag("Item_UI"))
            {
                // Verifico que no tome el hit con el objeto mismo que estoy arrastrando
                if (hitObj != this.gameObject)
                {
                    ItemUI hitObjItemData = hitObj.GetComponent<ItemUI>();
                    if (hitObjItemData.itemData.ID != id)
                    {
                        slot = hitObjItemData.transform.parent;
                        Inventory.Instance.UpdateParent(hitObjItemData, exParent);
                        break;
                    }
                    else
                    {
                        if (itemData.acumulable && hitObjItemData.quantity + quantity <= itemData.maxStack)
                        {
                            quantity += hitObjItemData.quantity;
                            slot = hitObjItemData.transform.parent;
                            //Inventory.Instance.DeleteItem(hitObjItemData, hitObjItemData.quantity, true);
                            break;
                        }
                        else
                        {
                            slot = hitObjItemData.transform.parent;
                            Inventory.Instance.UpdateParent(hitObjItemData, exParent);
                            break;
                        }
                    }
                }
            }
        }

        Inventory.Instance.UpdateParent(this, slot != null ? slot : exParent);
    }
}