using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Si arrastramos un item aquí, simplemente lo destruimos
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null)
        {
            Destroy(dropped);
            Debug.Log("Item eliminado en la basura");
        }
    }
}
