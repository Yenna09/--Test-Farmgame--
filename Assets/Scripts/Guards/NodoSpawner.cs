using UnityEngine;
using System.Collections;

public class NodoSpawner : MonoBehaviour
{
    [Header("Configuración del Nodo")]
    [Tooltip("El prefab del enemigo que va a nacer acá (ej: tu EnemyBear)")]
    public GameObject enemigoPrefab;
    
    [Tooltip("Hora en la que los enemigos deben spawnear")]
    public int horaSpawn = 21;
    
    [Tooltip("Tiempo máximo en segundos que tardará este nodo en reaccionar al anochecer")]
    public float retrasoMaximo = 2f; 
    
    [Tooltip("Arrastrá acá los objetos vacíos de la escena que formarán la ruta de este enemigo")]
    public Transform[] waypointsLocales;

    
    private GameObject enemigoInstanciado; 

    private void OnEnable()
    {
        // Nos suscribimos al cambio de tiempo para verificar la hora de spawn
        DayNightManager.AlCambiarTiempo += VerificarHoraSpawn;
        DayNightManager.AlAmanecer += LimpiarEnemigo;
    }

    private void OnDisable()
    {
        
        DayNightManager.AlCambiarTiempo -= VerificarHoraSpawn;
        DayNightManager.AlAmanecer -= LimpiarEnemigo;
    }

    private void VerificarHoraSpawn(int hora, int minuto)
    {
        // Validar que los waypoints estén asignados
        if (waypointsLocales == null || waypointsLocales.Length == 0)
        {
            Debug.LogWarning($"[NodoSpawner] El nodo {gameObject.name} no tiene waypoints asignados. Por favor, asignalos en el inspector.");
            return;
        }

        // Si llegamos a la hora de spawn exacta (con minuto 0)
        if (hora == horaSpawn && minuto == 0 && enemigoInstanciado == null)
        {
            StartCoroutine(RutinaSpawnConRetraso());
        }
    }

    private IEnumerator RutinaSpawnConRetraso()
    {
        float tiempoDeEspera = Random.Range(0f, retrasoMaximo);
        yield return new WaitForSeconds(tiempoDeEspera);

        // Doble verificación antes de instanciar
        if (enemigoInstanciado == null && waypointsLocales != null && waypointsLocales.Length > 0)
        {
            enemigoInstanciado = Instantiate(enemigoPrefab, transform.position, transform.rotation);
            
            EnemigoPatrol cerebroDelEnemigo = enemigoInstanciado.GetComponent<EnemigoPatrol>();
            
            if (cerebroDelEnemigo != null)
            {
                cerebroDelEnemigo.AsignarWaypoints(waypointsLocales);
            }
            else
            {
                Debug.LogWarning($"El enemigo instanciado en {gameObject.name} no tiene el script EnemigoPatrol.");
            }
        }
        else if (waypointsLocales == null || waypointsLocales.Length == 0)
        {
            Debug.LogError($"[NodoSpawner] No se puede spawnear enemigo en {gameObject.name}: waypoints no configurados.");
        }
    }

    private void LimpiarEnemigo()
    {
        
        if (enemigoInstanciado != null)
        {
            Destroy(enemigoInstanciado);
            
            
        }
    }
}