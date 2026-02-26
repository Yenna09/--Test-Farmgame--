using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRecogible : MonoBehaviour
{
    [Header("Configuración del Ítem")]
    [Tooltip("Este ID debe coincidir con el del archivo JSON")]
    public int itemID; // ✨ Aquí asignarás el número en Unity

    // Esta es la función que mencionaste para detectar al conejo
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Recolectar();
        }
    }

    private void Recolectar()
    {
        
        //Aquí avisaremos al Inventario usando nuestro itemID
        Debug.Log("Recogiendo ítem con ID: " + itemID);

        //Desaparece del mundo
        Destroy(gameObject);
    }
}
