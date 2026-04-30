using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public int itemID = 0; 
    public int quantity = 1;
    
    [Header("DNI Único en el Mundo")]
    [Tooltip("Escribí un nombre único para este ítem. Ej: Manzana_Granja_1")]
    public string uniqueWorldID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory.Instance.PickUpItem(itemID, quantity);
            
            // Avisamos al SaveController que anote este DNI
            if (SaveController.Instance != null && !string.IsNullOrEmpty(uniqueWorldID))
            {
                SaveController.Instance.destroyedItemsIDs.Add(uniqueWorldID);
            }

            Destroy(gameObject);
        }
    }

    // --- TRUCO DE DESARROLLADOR ---
    // Esto te crea un botón en el Inspector para generar IDs al azar y no tener que escribirlos a mano
    [ContextMenu("Generar DNI Aleatorio")]
    private void GenerateID()
    {
        uniqueWorldID = System.Guid.NewGuid().ToString();
    }
}
