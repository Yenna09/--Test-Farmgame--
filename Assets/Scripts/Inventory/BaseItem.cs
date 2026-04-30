using UnityEngine;

public class BaseItem : MonoBehaviour
{
    [SerializeField] private int itemID; 
    [SerializeField] private int quantity = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // LE AVISAMOS AL INVENTARIO (Mandamos solo los DATOS, no el objeto)
            Inventory.Instance.PickUpItem(itemID, quantity);
            
            // EL OBJETO DEL SUELO MUERE ACÁ
            Destroy(gameObject);
        }
    }
}
