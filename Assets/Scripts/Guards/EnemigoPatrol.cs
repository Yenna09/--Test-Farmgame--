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
    private SpriteRenderer spriteRenderer;

    
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
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        

        distanciaWaypoints2 = distanciaWaypoints * distanciaWaypoints;
    }

    // ¡Cambiamos LateUpdate por Update!
    private void Update() 
    {
        // Si no hay sprite, no hacemos nada para evitar errores
        if (spriteRenderer == null) return;

        // Leemos hacia dónde está caminando el NavMeshAgent
        float velocidadX = agent.velocity.x;

        // Si camina hacia la derecha (velocidad positiva)
        if (velocidadX > 0.05f)
        {
            spriteRenderer.flipX = false; 
        }
        // Si camina hacia la izquierda (velocidad negativa)
        else if (velocidadX < -0.05f)
        {
            spriteRenderer.flipX = true; 
        }
    }
    public override void IdleState()
    {
        base.IdleState();

        
        if (Waypoints == null || Waypoints.Length == 0)
        {
            agent.SetDestination(transform.position);
            return; 
        }

    
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
        agent.SetDestination(transform.position); 
        
        
        //if (target != null)
        //{
        //    Vector3 direccion = (target.position - transform.position).normalized;
        //    direccion.y = 0;
        //    transform.rotation = Quaternion.LookRotation(direccion);
        //}

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position);
        }
        
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