using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    [SerializeField]public Image image;
    UnityEngine.Vector3 dragOffset;
    [HideInInspector]public Transform parentAfterDrag;

    // En DragAndDrop.cs
    void Awake()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }
    // En DragAndDrop.cs
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.pointerDrag = null; // Esto "mata" el arrastre para el clic derecho
            return;
        }

        // IMPORTANTE: Calculá el offset aquí para que no salte la manzana
        dragOffset = transform.position - Input.mousePosition; 

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }



    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        // Si soltamos fuera de la UI (fuera de cualquier slot)
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // Aquí llamarías a una función para instanciar el objeto en el suelo
            Debug.Log("Soltado en el mundo");
        }
        else
        {
            // Lógica normal de volver al slot o cambiar de slot
            transform.SetParent(parentAfterDrag);
            
            RectTransform rect = GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
