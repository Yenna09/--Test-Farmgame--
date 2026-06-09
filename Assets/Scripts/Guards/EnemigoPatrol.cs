using UnityEngine;
using UnityEngine.AI;

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
    }

    public void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); 

        // --- BLINDAJE 2.5D DEFINITIVO ---
        if (agent != null)
        {
            agent.updateRotation = false; // No lo rota el NavMesh
            agent.updateUpAxis = false;   // No se inclina con el suelo
        }

        distanciaWaypoints2 = distanciaWaypoints * distanciaWaypoints;
    }

    // El espejo visual
    private void Update() 
    {
        if (spriteRenderer == null || agent == null) return;

        float velocidadX = agent.velocity.x;

        if (velocidadX > 0.05f)
        {
            spriteRenderer.flipX = false; 
        }
        else if (velocidadX < -0.05f)
        {
            spriteRenderer.flipX = true; 
        }
    }

    public override void IdleState()
    {
        base.IdleState();

        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        // Seguro por si no hay waypoints asignados
        if (Waypoints == null || Waypoints.Length == 0)
        {
            agent.SetDestination(transform.position);
            return; 
        }

        // Patrullaje
        agent.SetDestination(Waypoints[indice].position);
        if ((Waypoints[indice].position - transform.position).sqrMagnitude < distanciaWaypoints2)
        {
            indice = (indice + 1) % Waypoints.Length;
        }
    }

    public override void FollowState()
    {
        base.FollowState();
        
        if (target != null && agent.isActiveAndEnabled && agent.isOnNavMesh) 
        {
            agent.SetDestination(target.position);
        }
    }

    public override void AttackState()
    {
        base.AttackState();
        
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position); // Se detiene para golpear
        }

        // Ataque con cooldown
        if (Time.time >= tiempoSiguienteAtaque)
        {
            Atacar();
            tiempoSiguienteAtaque = Time.time + velocidadAtaque;
        }
    }

    public override void DeathState()
    {
        base.IdleState();
        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    public void Atacar()
    {
        if (Personaje.singleton != null && Personaje.singleton.vida != null)
        {
            Personaje.singleton.vida.CausarDamage(damage);
        }
    }
}