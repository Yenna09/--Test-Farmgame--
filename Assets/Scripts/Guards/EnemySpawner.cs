using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemigoPrefab;
    public Transform puntoDeSpawn;
    public float tiempoEntreSpawns = 3f;

    private Coroutine rutinaSpawneo;

    // Nos suscribimos a los eventos cuando el objeto se activa
    private void OnEnable()
    {
        DayNightManager.AlAnochecer += IniciarInvasion;
        DayNightManager.AlAmanecer += DetenerInvasion;
    }

    // Es CRÍTICO desuscribirse cuando el objeto se desactiva para evitar memory leaks
    private void OnDisable()
    {
        DayNightManager.AlAnochecer -= IniciarInvasion;
        DayNightManager.AlAmanecer -= DetenerInvasion;
    }

    private void IniciarInvasion()
    {
        // Iniciamos una corrutina que spawnea enemigos de forma cíclica
        if (rutinaSpawneo == null)
        {
            rutinaSpawneo = StartCoroutine(RutinaGenerarEnemigos());
        }
    }

    private void DetenerInvasion()
    {
        // Detenemos la corrutina al amanecer
        if (rutinaSpawneo != null)
        {
            StopCoroutine(rutinaSpawneo);
            rutinaSpawneo = null;
        }
        
        // Opcional: Aquí podrías buscar a todos los enemigos vivos y destruirlos
        // o hacer que huyan bajo tierra.
    }

    private IEnumerator RutinaGenerarEnemigos()
    {
        // Bucle infinito que solo corre mientras es de noche
        while (true)
        {
            Instantiate(enemigoPrefab, puntoDeSpawn.position, puntoDeSpawn.rotation);
            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }
}