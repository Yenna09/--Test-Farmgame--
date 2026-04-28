using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    // En InventorySlot.cs
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return; //

        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        ItemUI droppedItem = dropped.GetComponent<ItemUI>();
        
        // Lógica de unificación que ya tenés...
        if (transform.childCount > 0)
        {
            ItemUI targetItem = transform.GetChild(0).GetComponent<ItemUI>();
            if (targetItem.id == droppedItem.id && targetItem.itemData.acumulable)
            {
                if (targetItem.quantity + droppedItem.quantity <= targetItem.itemData.maxStack)
                {
                    targetItem.quantity += droppedItem.quantity;
                    targetItem.RefreshUI();
                    Destroy(dropped); 
                    return;
                }
            }
        }
        dropped.GetComponent<DragAndDrop>().parentAfterDrag = transform;
    }
}