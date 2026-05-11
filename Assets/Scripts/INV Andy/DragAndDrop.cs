using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    [SerializeField] public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    
    private Vector3 dragOffset;
    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup;
    
    // NUEVO: Guardamos de dónde salimos para saber si hay que volver a unir el stack
    private Transform originalParent; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Cancelamos si intentan arrastrar con la ruedita del mouse
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            eventData.pointerDrag = null;
            return;
        }

        parentAfterDrag = transform.parent;
        originalParent = parentAfterDrag; // Guardamos el slot de origen
        ItemUI itemUI = GetComponent<ItemUI>();

        // --- MAGIA DE DIVIDIR (CLIC DERECHO) ---
        if (eventData.button == PointerEventData.InputButton.Right && itemUI != null && itemUI.quantity > 1)
        {
            int cantidadParaDejar = itemUI.quantity / 2;
            int cantidadParaLlevar = itemUI.quantity - cantidadParaDejar;

            // 1. Creamos un clon para dejar en el slot
            GameObject clone = Instantiate(Inventory.Instance.itemPrefab.gameObject, originalParent);
            clone.name = "Item_UI_ID_" + itemUI.id;

            ItemUI cloneUI = clone.GetComponent<ItemUI>();
            if (cloneUI != null) cloneUI.InitializeItem(itemUI.id, cantidadParaDejar);

            // Ajustamos el tamaño del clon para que no se vea roto
            RectTransform cloneRect = clone.GetComponent<RectTransform>();
            if (cloneRect != null)
            {
                cloneRect.anchorMin = Vector2.zero;
                cloneRect.anchorMax = Vector2.one;
                cloneRect.offsetMin = Vector2.zero;
                cloneRect.offsetMax = Vector2.zero;
                cloneRect.localPosition = Vector3.zero;
                cloneRect.localScale = Vector3.one;
            }

            // 2. Modificamos el que nos estamos llevando en el mouse
            itemUI.quantity = cantidadParaLlevar;
            itemUI.RefreshUI();
        }

        // --- ARRASTRE NORMAL ---
        dragOffset = transform.position - Input.mousePosition; 
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Si el objeto se fusionó con éxito en OnDrop, Unity lo marcó para destruir.
        if (this == null || gameObject == null) return;

        // SISTEMA ANTI-DUPLICADOS: 
        // Si cancelaste el arrastre (volviste al slot original) y ahí dejamos un clon...
        if (parentAfterDrag == originalParent && parentAfterDrag.childCount > 0)
        {
            ItemUI itemInSlot = parentAfterDrag.GetChild(0).GetComponent<ItemUI>();
            ItemUI myItemUI = GetComponent<ItemUI>();

            // ...los volvemos a fusionar automáticamente.
            if (itemInSlot != null && itemInSlot != myItemUI && itemInSlot.id == myItemUI.id)
            {
                itemInSlot.quantity += myItemUI.quantity;
                itemInSlot.RefreshUI();
                Destroy(gameObject); // Yo me destruyo, el clon asume todo
                return;
            }
        }

        // Si soltaste en un slot válido
        transform.SetParent(parentAfterDrag);
        canvasGroup.blocksRaycasts = true; 

        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }
    }
}