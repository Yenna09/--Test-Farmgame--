using UnityEngine;
using UnityEngine.AI;

public class EnemigoPatrol : Enemy
{
    private NavMeshAgent agent;
    
    // CAMBIO 1: Guardamos las posiciones como Vector3, no como Transform
    public Vector3[] waypointsPositions; 
    private int indice;
    public float distanciaWaypoints;
    private float distanciaWaypoints2;
    public float damage = 3;
    public float tiempoEsperaRetorno = 1f;
    private float tiempoRetorno;

    private float tiempoSiguienteAtaque;
    public float velocidadAtaque = 1.5f;
    
    private SpriteRenderer spriteRenderer;

    // CAMBIO 2: Extraemos las posiciones exactas de los Transforms
    public void AsignarWaypoints(Transform[] rutasDelSpawner)
    {
        if (rutasDelSpawner == null || rutasDelSpawner.Length == 0) return;

        waypointsPositions = new Vector3[rutasDelSpawner.Length];
        for (int i = 0; i < rutasDelSpawner.Length; i++)
        {
            waypointsPositions[i] = rutasDelSpawner[i].position;
        }
    }

    // CAMBIO 3: Devuelve Vector3 en lugar de Transform
    private bool TryGetNextValidWaypoint(out Vector3 waypoint)
    {
        waypoint = Vector3.zero;
        if (waypointsPositions == null || waypointsPositions.Length == 0) return false;

        waypoint = waypointsPositions[indice];
        return true;
    }

    // CAMBIO 4: Buscamos el ÍNDICE del waypoint más cercano basándonos en los Vector3
    private int GetClosestWaypointIndex()
    {
        if (waypointsPositions == null || waypointsPositions.Length == 0) return 0;

        int closestIndex = 0;
        float bestSqrDist = float.MaxValue;

        for (int i = 0; i < waypointsPositions.Length; i++)
        {
            float sqrDist = (waypointsPositions[i] - transform.position).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                closestIndex = i;
            }
        }

        return closestIndex;
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

        // Verificamos si tenemos coordenadas válidas guardadas
        if (!TryGetNextValidWaypoint(out Vector3 siguienteWaypoint))
        {
            Debug.LogWarning($"[EnemigoPatrol] No hay waypoints válidos para {gameObject.name}.");
            agent.SetDestination(transform.position);
            return;
        }

        // CAMBIO 5: Si el jugador desaparece (cambio de escena), recalcula el waypoint más cercano a su posición actual
        if (target == null)
        {
            indice = GetClosestWaypointIndex();
            Vector3 waypointCercano = waypointsPositions[indice];
            
            agent.SetDestination(waypointCercano);
            
            if ((waypointCercano - transform.position).sqrMagnitude < distanciaWaypoints2)
            {
                indice = (indice + 1) % waypointsPositions.Length;
            }
            return;
        }

        // Patrullaje normal
        agent.SetDestination(siguienteWaypoint);
        if ((siguienteWaypoint - transform.position).sqrMagnitude < distanciaWaypoints2)
        {
            indice = (indice + 1) % waypointsPositions.Length;
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