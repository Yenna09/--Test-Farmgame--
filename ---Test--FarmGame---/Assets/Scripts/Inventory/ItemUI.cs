using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class ItemUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int id;
    public int quantity;

    [HideInInspector] public Database.InventoryItem itemData;
    [HideInInspector] public Transform exParent;

    public GameObject deleteButton;
    private TextMeshProUGUI quantityText;
    private Image iconoImage;
    private Vector3 dragOffset;

    void Awake() 
    {
        quantityText = GetComponentInChildren<TextMeshProUGUI>();
        iconoImage = GetComponent<Image>();
        exParent = transform.parent;
        
        if (exParent != null && exParent.GetComponent<Image>() != null) 
            exParent.GetComponent<Image>().fillCenter = true;

        InitializeItem(id, quantity);
    }

    void Update()
    {
        if (quantityText != null) quantityText.text = quantity.ToString();
    }

    public void InitializeItem(int _id, int _quantity)
    {
        id = _id;
        quantity = _quantity;
        var database = Inventory.Instance.db;
        
        if (database != null && id < database.dataBase.Length)
        {
            itemData = database.dataBase[id];
            iconoImage.sprite = itemData.icono;
        }
        if(deleteButton != null) deleteButton.SetActive(false);
    }

    public void EnableDeletion(bool enable) { if(deleteButton != null) deleteButton.SetActive(enable); }
    public void Delete() { Inventory.Instance.DeleteItem(this, quantity, true); }
    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (quantityText != null) quantityText.enabled = false;
        exParent = transform.parent;
        if (exParent.GetComponent<Image>()) exParent.GetComponent<Image>().fillCenter = false;
        transform.SetParent(Inventory.Instance.transform);
        dragOffset = transform.position - Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData) { transform.position = Input.mousePosition + dragOffset; }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (quantityText != null) quantityText.enabled = true;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        Transform slot = null;
        
        Inventory.Instance.graphRay.Raycast(eventData, raycastResults);

        foreach(RaycastResult hit in raycastResults)
        {
            var hitObj = hit.gameObject; 
            if (hitObj.CompareTag("Slot") && hitObj.transform.childCount == 0)
            {
                slot = hitObj.transform;
                break;
            }
            if (hitObj.CompareTag("Item_UI") && hitObj != this.gameObject)
            {
                ItemUI hitObjItemData = hitObj.GetComponent<ItemUI>();
                if (hitObjItemData.itemData.ID != id)
                {
                    slot = hitObjItemData.transform.parent;
                    Inventory.Instance.UpdateParent(hitObjItemData, exParent);
                    break;
                }
                else if (itemData.acumulable && hitObjItemData.quantity + quantity <= itemData.maxStack)
                {
                    quantity += hitObjItemData.quantity;
                    slot = hitObjItemData.transform.parent;
                    Inventory.Instance.DeleteItem(hitObjItemData, hitObjItemData.quantity, true);
                    break;
                }
            }
        }
        Inventory.Instance.UpdateParent(this, slot != null ? slot : exParent);
    }
}