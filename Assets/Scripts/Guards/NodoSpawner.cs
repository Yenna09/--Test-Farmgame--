using UnityEngine;
using System.Collections;

public class NodoSpawner : MonoBehaviour
{
    [Header("Configuración del Nodo")]
    [Tooltip("El prefab del enemigo que va a nacer acá (ej: tu EnemyBear)")]
    public GameObject enemigoPrefab;
    
    [Tooltip("Hora en la que los enemigos deben spawnear")]
    public int horaSpawn = 21;
    
    [Header("Estado de persistencia")]
    [Tooltip("Si este nodo ya creó un enemigo y debe mantenerse entre escenas")]
    public bool spawnedEnemies = false;

    private static readonly System.Collections.Generic.Dictionary<string, GameObject> persistedEnemies = new System.Collections.Generic.Dictionary<string, GameObject>();
    private string spawnKey;

    [Tooltip("Tiempo máximo en segundos que tardará este nodo en reaccionar al anochecer")]
    public float retrasoMaximo = 2f; 
    
    [Tooltip("Arrastrá acá los objetos vacíos de la escena que formarán la ruta de este enemigo")]
    public Transform[] waypointsLocales;

    
    private GameObject enemigoInstanciado; 

    private void Awake()
    {
        spawnKey = GetSpawnKey();
    }

    private string GetSpawnKey()
    {
        Vector3 pos = transform.position;
        string sceneName = gameObject.scene.name;
        return $"{sceneName}_{pos.x:F2}_{pos.y:F2}_{pos.z:F2}";
    }

    private void OnEnable()
    {
        // Nos suscribimos al cambio de tiempo para verificar la hora de spawn
        DayNightManager.AlCambiarTiempo += VerificarHoraSpawn;
        DayNightManager.AlAmanecer += LimpiarEnemigo;
    }

    private void Start()
    {
        TryRestoreEnemy();
        TrySpawnIfNightAlready();
    }

    private void OnDisable()
    {
        DayNightManager.AlCambiarTiempo -= VerificarHoraSpawn;
        DayNightManager.AlAmanecer -= LimpiarEnemigo;
    }

    private void VerificarHoraSpawn(int hora, int minuto)
    {
        if (spawnedEnemies) return;

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

    public void RestaurarEstado(bool spawned)
    {
        spawnedEnemies = spawned;
        if (!spawnedEnemies) return;

        TryRestoreEnemy();
    }

    private void TryRestoreEnemy()
    {
        if (persistedEnemies.TryGetValue(spawnKey, out GameObject enemigoExistente) && enemigoExistente != null)
        {
            enemigoInstanciado = enemigoExistente;
            spawnedEnemies = true;
        }
    }

    private void TrySpawnIfNightAlready()
    {
        if (spawnedEnemies || enemigoInstanciado != null) return;
        if (DayNightManager.Instance == null) return;

        if (!DayNightManager.Instance.esDeDia)
        {
            if (waypointsLocales == null || waypointsLocales.Length == 0)
            {
                Debug.LogWarning($"[NodoSpawner] El nodo {gameObject.name} no tiene waypoints asignados. Por favor, asignalos en el inspector.");
                return;
            }

            StartCoroutine(RutinaSpawnConRetraso());
        }
    }

    private void InstanciarEnemigo()
    {
        if (waypointsLocales == null || waypointsLocales.Length == 0)
        {
            Debug.LogError($"[NodoSpawner] No se puede restaurar el enemigo en {gameObject.name}: waypoints no configurados.");
            return;
        }

        enemigoInstanciado = Instantiate(enemigoPrefab, transform.position, transform.rotation);
        DontDestroyOnLoad(enemigoInstanciado);

        if (!persistedEnemies.ContainsKey(spawnKey))
        {
            persistedEnemies[spawnKey] = enemigoInstanciado;
        }

        EnemigoPatrol cerebroDelEnemigo = enemigoInstanciado.GetComponent<EnemigoPatrol>();
        if (cerebroDelEnemigo != null)
        {
            Transform[] rutasValidas = GetValidWaypoints(waypointsLocales);
            if (rutasValidas.Length != waypointsLocales.Length)
            {
                Debug.LogWarning($"[NodoSpawner] El nodo {gameObject.name} tiene waypoints nulos. Se ignorarán entradas vacías.");
            }

            if (rutasValidas.Length == 0)
            {
                Debug.LogError($"[NodoSpawner] No se puede instanciar el enemigo en {gameObject.name}: no hay waypoints válidos.");
                Destroy(enemigoInstanciado);
                enemigoInstanciado = null;
                return;
            }

            cerebroDelEnemigo.AsignarWaypoints(rutasValidas);
        }
        else
        {
            Debug.LogWarning($"El enemigo instanciado en {gameObject.name} no tiene el script EnemigoPatrol.");
        }
    }

    private Transform[] GetValidWaypoints(Transform[] rutas)
    {
        if (rutas == null || rutas.Length == 0) return new Transform[0];

        int count = 0;
        foreach (Transform waypoint in rutas)
        {
            if (waypoint != null) count++;
        }

        Transform[] validWaypoints = new Transform[count];
        int index = 0;
        foreach (Transform waypoint in rutas)
        {
            if (waypoint != null)
            {
                validWaypoints[index++] = waypoint;
            }
        }

        return validWaypoints;
    }

    private IEnumerator RutinaSpawnConRetraso()
    {
        float tiempoDeEspera = Random.Range(0f, retrasoMaximo);
        yield return new WaitForSeconds(tiempoDeEspera);

        // Doble verificación antes de instanciar
        if (enemigoInstanciado == null && waypointsLocales != null && waypointsLocales.Length > 0)
        {
            InstanciarEnemigo();
            spawnedEnemies = true;
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
            persistedEnemies.Remove(spawnKey);
            Destroy(enemigoInstanciado);
            enemigoInstanciado = null;
        }
        spawnedEnemies = false;
    }
}

