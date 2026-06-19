using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodoSpawner : MonoBehaviour
{
    #region Configuration
    [SerializeField] private GameObject enemigoPrefab;
    [SerializeField] private int horaSpawn = 21;
    [SerializeField] private float retrasoMaximo = 2f;
    [SerializeField] private Transform[] waypointsLocales;
    #endregion

    #region Persistence Status
    [SerializeField] private bool spawnedEnemies = false;
    private static readonly Dictionary<string, GameObject> persistedEnemies = new Dictionary<string, GameObject>();
    private string spawnKey;
    private GameObject enemigoInstanciado;
    #endregion

    private void Awake()
    {
        spawnKey = GetSpawnKey();
    }

    private void OnEnable()
    {
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

    private string GetSpawnKey()
    {
        Vector3 pos = transform.position;
        string sceneName = gameObject.scene.name;
        return $"{sceneName}_{pos.x:F2}_{pos.y:F2}_{pos.z:F2}";
    }

    private void VerificarHoraSpawn(int hora, int minuto)
    {
        if (spawnedEnemies) return;

        if (waypointsLocales == null || waypointsLocales.Length == 0)
        {
            return;
        }

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
        
        if (!DayNightManager.esDeDia)
        {
            if (waypointsLocales == null || waypointsLocales.Length == 0)
            {
                return;
            }

            StartCoroutine(RutinaSpawnConRetraso());
        }
    }

    private void InstanciarEnemigo()
    {
        if (waypointsLocales == null || waypointsLocales.Length == 0)
        {
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

            if (rutasValidas.Length == 0)
            {
                Destroy(enemigoInstanciado);
                enemigoInstanciado = null;
                return;
            }

            cerebroDelEnemigo.ConstruirGrafo(rutasValidas);
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

        if (enemigoInstanciado == null && waypointsLocales != null && waypointsLocales.Length > 0)
        {
            InstanciarEnemigo();
            spawnedEnemies = true;
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