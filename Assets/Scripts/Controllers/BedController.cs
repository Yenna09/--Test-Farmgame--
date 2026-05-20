using UnityEngine;

public class BedController : MonoBehaviour
{
    [Header("UI del Menú de Dormir")]
    public GameObject panelDormirUI;

    private bool jugadorCerca = false;
    private PlayerMovement playerMovement;

    void Start()
    {
        if (panelDormirUI != null) panelDormirUI.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E) && playerMovement != null)
        {
            if (!panelDormirUI.activeSelf) 
            {
                AbrirMenu();
            }
        }
    }

    void AbrirMenu()
    {
        panelDormirUI.SetActive(true);
        
        if (playerMovement != null)
        {
            playerMovement.estaUsandoHerramienta = true; 
            Rigidbody rb = playerMovement.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }
    }

    public void CerrarMenu()
    {
        panelDormirUI.SetActive(false);
        
        if (playerMovement != null)
        {
            playerMovement.estaUsandoHerramienta = false;
        }
    }

    // --- FUNCIONES DE LOS BOTONES INSTANTÁNEAS ---

    public void OpcionDormirHastaNoche()
    {
        DayNightManager manager = FindObjectOfType<DayNightManager>();
        if (manager != null) manager.DormirHasta(manager.horaAnochecer, false);
        
        CerrarMenu(); // Cerramos el menú y devolvemos el control al instante
    }

    public void OpcionDormirHastaAmanecer()
    {
        DayNightManager manager = FindObjectOfType<DayNightManager>();
        if (manager != null) manager.DormirHasta(manager.horaAmanecer, true);
        
        CerrarMenu(); // Cerramos el menú y devolvemos el control al instante
    }

    // --- DETECCIÓN DEL JUGADOR ---

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            playerMovement = other.GetComponent<PlayerMovement>();
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            CerrarMenu();
        }
    }
}