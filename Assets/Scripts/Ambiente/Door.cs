using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Tooltip("Nombre de la escena a cargar (Ej: 'InteriorCasa' o 'MundoPrincipal')")]
    public string sceneToLoad;

    [Tooltip("TILDAR SOLO EN LA PUERTA DE ADENTRO (para avisar que salimos)")]
    public bool salirAlMundo = false;

    public static bool saliendoDeCasa = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (salirAlMundo)
            {
                saliendoDeCasa = true;
            }

           
            // Le decimos al SaveController que guarde el inventario (y todo lo demás)
            // justo antes de que Unity destruya la escena.
            if (SaveController.Instance != null)
            {
                SaveController.Instance.SaveGame();
            }
            
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}