using UnityEngine;
using System.Collections;

public class NodoSpawner : MonoBehaviour
{
    [Header("Configuración del Nodo")]
    [Tooltip("El prefab del enemigo que va a nacer acá (ej: tu EnemyBear)")]
    public GameObject enemigoPrefab;
    
    [Tooltip("Tiempo máximo en segundos que tardará este nodo en reaccionar al anochecer")]
    public float retrasoMaximo = 2f; 
    
    [Tooltip("Arrastrá acá los objetos vacíos de la escena que formarán la ruta de este enemigo")]
    public Transform[] waypointsLocales;

    
    private GameObject enemigoInstanciado; 

    private void OnEnable()
    {
        // Nos suscribimos al cerebro del tiempo
        DayNightManager.AlAnochecer += IniciarDespertar;
        DayNightManager.AlAmanecer += LimpiarEnemigo;
    }

    private void OnDisable()
    {
        
        DayNightManager.AlAnochecer -= IniciarDespertar;
        DayNightManager.AlAmanecer -= LimpiarEnemigo;
    }

    private void IniciarDespertar()
    {
        
        if (enemigoInstanciado == null)
        {
            StartCoroutine(RutinaSpawnConRetraso());
        }
    }

    private IEnumerator RutinaSpawnConRetraso()
    {
        
        float tiempoDeEspera = Random.Range(0f, retrasoMaximo);
        yield return new WaitForSeconds(tiempoDeEspera);

        
        if (enemigoInstanciado == null)
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
    }

    private void LimpiarEnemigo()
    {
        
        if (enemigoInstanciado != null)
        {
            Destroy(enemigoInstanciado);
            
            
        }
    }
}