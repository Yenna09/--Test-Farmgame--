using System.Collections;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    [Tooltip("TILDAR SOLO EN EL FELPUDO DE AFUERA (en el mundo)")]
    public bool SpawnDelMundo = false;

    // Al poner IEnumerator, convertimos el Start en una rutina que puede "esperar"
    IEnumerator Start()
    {
        // Le damos 1 frame de ventaja al SaveController para que termine de acomodar todo
        yield return null; 

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Caso A: Estamos en el Mundo y venimos desde la casa
            if (SpawnDelMundo && Door.saliendoDeCasa)
            {
                player.transform.position = transform.position;
                Door.saliendoDeCasa = false; // Reseteamos la alarma
            }
            // Caso B: Estamos adentro de la Casa
            else if (!SpawnDelMundo)
            {
                player.transform.position = transform.position;
            }
        }
    }
}
