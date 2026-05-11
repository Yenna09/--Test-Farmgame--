using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoPatrol : Enemy
{
    private NavMeshAgent agent;
    public Transform[] Waypoints;
    private int indice;
    public float distanciaWaypoints;
    private float distanciaWaypoints2;
    public float damage = 3;

    private float tiempoSiguienteAtaque;
    public float velocidadAtaque = 1.5f;

    // --- ¡EL ENCHUFE! ---
    // El NodoSpawner va a llamar a esta función justo después de instanciarlo
    public void AsignarWaypoints(Transform[] rutasDelSpawner)
    {
        Waypoints = rutasDelSpawner;
        Debug.Log("¡EnemigoPatrol recibió su ruta de patrullaje!");
    }

    public void Awake() 
    {
        // Esto llama al Awake() de Enemy.cs, que ahora es el encargado
        // de buscar al jugador de forma totalmente segura.
        base.Awake(); 
        
        agent = GetComponent<NavMeshAgent>();
        distanciaWaypoints2 = distanciaWaypoints * distanciaWaypoints;
    }
    public override void IdleState()
    {
        base.IdleState();

        // --- SEGURO DE VIDA ---
        // Si el Spawner todavía no nos pasó la ruta, o si nos olvidamos de
        // ponerle waypoints al Spawner, el enemigo simplemente se queda quieto.
        if (Waypoints == null || Waypoints.Length == 0)
        {
            agent.SetDestination(transform.position);
            return; // Cortamos la ejecución acá para que no dé error
        }

        // Tu lógica original (¡que está perfecta!)
        agent.SetDestination(Waypoints[indice].position);
        if ((Waypoints[indice].position - transform.position).sqrMagnitude < distanciaWaypoints2)
        {
            indice = (indice + 1) % Waypoints.Length;
        }
    }

    public override void FollowState()
    {
        base.FollowState();
        
        // ¡SEGURO CONTRA CRASHEOS!
        if (target != null) 
        {
            agent.SetDestination(target.position);
        }
    }

    public override void AttackState()
    {
        base.AttackState();
        agent.SetDestination(transform.position); // Se detiene
        
        // ¡SEGURO CONTRA CRASHEOS!
        if (target != null)
        {
            // Mirar al jugador (Eje Y solamente para que no se incline)
            Vector3 direccion = (target.position - transform.position).normalized;
            direccion.y = 0;
            transform.rotation = Quaternion.LookRotation(direccion);
        }

        // Lógica de daño por tiempo
        if (Time.time >= tiempoSiguienteAtaque)
        {
            Atacar();
            tiempoSiguienteAtaque = Time.time + velocidadAtaque;
        }
    }

    public override void DeathState()
    {
        base.IdleState();
        agent.enabled = false;
    }

    public void Atacar()
    {
        Personaje.singleton.vida.CausarDamage(damage);
    }
}