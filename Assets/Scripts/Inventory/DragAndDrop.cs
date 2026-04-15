using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Para IBeginDragHandler y IDragHandler

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    Vector3 dragOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = transform.position - Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + dragOffset;
    }
}
