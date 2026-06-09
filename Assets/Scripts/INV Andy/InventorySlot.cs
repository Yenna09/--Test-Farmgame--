using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {

        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        ItemUI droppedItem = dropped.GetComponent<ItemUI>();
        DragAndDrop dragItem = dropped.GetComponent<DragAndDrop>();
        
        if (droppedItem == null || dragItem == null) return;

        //Lógica de unificación (Stack)
        if (transform.childCount > 0)
        {
            ItemUI targetItem = transform.GetChild(0).GetComponent<ItemUI>();
            
            //Obtenemos los datos del item destino desde la Base de Datos
            var targetData = Inventory.Instance.db.dataBase[targetItem.id];

            // Si son exactamente el mismo item y es acumulable
            if (targetItem.id == droppedItem.id && targetData.acumulable)
            {
                if (targetItem.quantity + droppedItem.quantity <= targetData.maxStack)
                {
                    targetItem.quantity += droppedItem.quantity;
                    targetItem.RefreshUI();
                    
                    Destroy(dropped); // Destruimos el que veníamos arrastrando
                    return;
                }
            }
            
            return; 
        }

        //Si el slot está vacío, le avisamos al script DragAndDrop 
        // cuál es su nuevo padre definitivo.
        dragItem.parentAfterDrag = transform;
    }
}