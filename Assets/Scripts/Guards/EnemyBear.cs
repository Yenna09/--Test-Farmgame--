using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBear : Enemy
{
    private NavMeshAgent agent;
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
        agent.SetDestination(transform.position);
        transform.LookAt(target, Vector3.up);
    }

    public override void DeathState()
    {
        base.IdleState();
        agent.enabled = false;
    }
}
