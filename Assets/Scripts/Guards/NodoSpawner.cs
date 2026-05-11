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

    // Referencia interna para saber si nuestro enemigo sigue vivo
    private GameObject enemigoInstanciado; 

    private void OnEnable()
    {
        // Nos suscribimos al cerebro del tiempo
        DayNightManager.AlAnochecer += IniciarDespertar;
        DayNightManager.AlAmanecer += LimpiarEnemigo;
    }

    private void OnDisable()
    {
        // Nos desuscribimos por seguridad cuando este nodo se apaga o destruye
        DayNightManager.AlAnochecer -= IniciarDespertar;
        DayNightManager.AlAmanecer -= LimpiarEnemigo;
    }

    private void IniciarDespertar()
    {
        // Si no hay un enemigo vivo de la noche anterior, arrancamos la corrutina
        if (enemigoInstanciado == null)
        {
            StartCoroutine(RutinaSpawnConRetraso());
        }
    }

    private IEnumerator RutinaSpawnConRetraso()
    {
        // 1. Calculamos un retraso aleatorio para que no aparezcan todos de golpe
        float tiempoDeEspera = Random.Range(0f, retrasoMaximo);
        yield return new WaitForSeconds(tiempoDeEspera);

        // 2. Comprobamos nuevamente que se pueda instanciar (por si amaneció durante la espera)
        if (enemigoInstanciado == null)
        {
            // 3. Instanciamos el prefab exactamente en la posición y rotación de este Nodo
            enemigoInstanciado = Instantiate(enemigoPrefab, transform.position, transform.rotation);
            
            // 4. Buscamos el script principal en el enemigo que acabamos de crear
            EnemigoPatrol cerebroDelEnemigo = enemigoInstanciado.GetComponent<EnemigoPatrol>();
            
            // 5. Si lo encontramos, le inyectamos los waypoints locales
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
        // Al amanecer, si el enemigo sigue vivo, lo destruimos
        if (enemigoInstanciado != null)
        {
            Destroy(enemigoInstanciado);
            
            // Opcional: Acá podrías instanciar un prefab de humo/partículas
            // Instantiate(efectoHumo, enemigoInstanciado.transform.position, Quaternion.identity);
        }
    }
}