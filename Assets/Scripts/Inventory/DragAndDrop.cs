using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Para IBeginDragHandler y IDragHandler

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    UnityEngine.Vector3 dragOffset;
    Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        throw new System.NotImplementedException();

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition + dragOffset;
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        Debug.Log("End Drag");
        throw new System.NotImplementedException();
    }
}
