using UnityEngine;
using UnityEngine.AI;

public class EnemyBear : Enemy
{
    private NavMeshAgent agent;
    public Transform[] Waypoints;
    private int indice;
    public float distanciaWaypoints;
    private float distanciaWaypoints2;
    public float damage = 3;
    public float tiempoEsperaRetorno = 1f;
    private float tiempoRetorno;

    private float tiempoSiguienteAtaque;
    public float velocidadAtaque = 1.5f;
    
    private SpriteRenderer spriteRenderer;

    public void AsignarWaypoints(Transform[] rutasDelSpawner)
    {
        Waypoints = rutasDelSpawner;
    }

    private bool TryGetNextValidWaypoint(out Transform waypoint)
    {
        waypoint = null;
        if (Waypoints == null || Waypoints.Length == 0) return false;

        int checks = 0;
        while (checks < Waypoints.Length)
        {
            if (Waypoints[indice] != null)
            {
                waypoint = Waypoints[indice];
                return true;
            }

            indice = (indice + 1) % Waypoints.Length;
            checks++;
        }

        return false;
    }

    private Transform GetClosestValidWaypoint()
    {
        if (Waypoints == null || Waypoints.Length == 0) return null;

        Transform closest = null;
        float bestSqrDist = float.MaxValue;

        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (Waypoints[i] == null) continue;

            float sqrDist = (Waypoints[i].position - transform.position).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                closest = Waypoints[i];
            }
        }

        return closest;
    }

    private int GetWaypointIndex(Transform waypoint)
    {
        if (Waypoints == null) return 0;

        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (Waypoints[i] == waypoint) return i;
        }

        return 0;
    }

    public void Awake() 
    {
        // Ya no llamamos a base.Awake() porque el padre no lo necesita
        
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); 

        // --- BLINDAJE 2.5D DEFINITIVO ---
        if (agent != null)
        {
            agent.updateRotation = false; // El NavMesh no rota el modelo 3D
            agent.updateUpAxis = false;   // Ignora las rampas/inclinaciones del suelo
        }

        distanciaWaypoints2 = distanciaWaypoints * distanciaWaypoints;
    }

    // El espejo visual (Reemplaza a las rotaciones 3D)
    private void Update() 
    {
        if (spriteRenderer == null || agent == null) return;

        float velocidadX = agent.velocity.x;

        // Si camina hacia la derecha
        if (velocidadX > 0.05f)
        {
            spriteRenderer.flipX = false; 
        }
        // Si camina hacia la izquierda
        else if (velocidadX < -0.05f)
        {
            spriteRenderer.flipX = true; 
        }
    }

    public override void IdleState()
    {
        base.IdleState();

        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        if (target != null)
        {
            tiempoRetorno = 0f;
        }
        else
        {
            if (tiempoRetorno <= 0f) tiempoRetorno = Time.time + tiempoEsperaRetorno;
            if (Time.time < tiempoRetorno)
            {
                agent.SetDestination(transform.position);
                return;
            }
        }

        // Seguro por si el Spawner no le asignó Waypoints o quedaron referencias nulas
        if (!TryGetNextValidWaypoint(out Transform siguienteWaypoint))
        {
            Debug.LogWarning($"[EnemyBear] No hay waypoints válidos para {gameObject.name}. Asigne rutas completas en el inspector.");
            agent.SetDestination(transform.position);
            return;
        }

        // Si no hay jugador presente en escena, volvemos al waypoint más cercano y sincronizamos el índice.
        if (target == null)
        {
            Transform waypointCercano = GetClosestValidWaypoint();
            if (waypointCercano != null)
            {
                indice = GetWaypointIndex(waypointCercano);
                agent.SetDestination(waypointCercano.position);
                if ((waypointCercano.position - transform.position).sqrMagnitude < distanciaWaypoints2)
                {
                    indice = (indice + 1) % Waypoints.Length;
                }
                return;
            }
        }

        // Lógica de patrullaje
        agent.SetDestination(siguienteWaypoint.position);
        if ((siguienteWaypoint.position - transform.position).sqrMagnitude < distanciaWaypoints2)
        {
            indice = (indice + 1) % Waypoints.Length;
        }
    }

    public override void FollowState()
    {
        base.FollowState();
        
        // Persecución limpia
        if (target != null && agent.isActiveAndEnabled && agent.isOnNavMesh) 
        {
            agent.SetDestination(target.position);
        }
    }

    public override void AttackState()
    {
        base.AttackState();
        
        // Frena para no atravesar al jugador
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position); 
        }

        // Ataque por tiempo (Cooldown)
        if (Time.time >= tiempoSiguienteAtaque)
        {
            Atacar();
            tiempoSiguienteAtaque = Time.time + velocidadAtaque;
        }
    }

    public override void DeathState()
    {
        base.IdleState(); // Forzamos a que deje de seguir/atacar visualmente
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