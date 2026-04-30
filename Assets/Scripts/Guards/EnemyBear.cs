using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBear : Enemy
{
    private NavMeshAgent agent;
    public float damage = 3;

    private float tiempoSiguienteAtaque;
    public float velocidadAtaque = 1.5f;
    public void Awake() 
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    public override void IdleState()
    {
        base.IdleState();
        agent.SetDestination(transform.position);
    }

    public override void FollowState()
    {
        base.FollowState();
        agent.SetDestination(target.position);
    }

    public override void AttackState()
    {
        base.AttackState();
        agent.SetDestination(transform.position); // Se detiene
        
        // Mirar al jugador (Eje Y solamente para que no se incline)
        Vector3 direccion = (target.position - transform.position).normalized;
        direccion.y = 0;
        transform.rotation = Quaternion.LookRotation(direccion);

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
