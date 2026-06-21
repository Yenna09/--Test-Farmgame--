using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : MonoBehaviour
{
    public States state;
    public Transform target;
    public bool vivo = true;
    public float distancia;

    [SerializeField] protected float distanceFollow = 10f;
    [SerializeField] protected float distanciaAtacar = 2f;
    [SerializeField] protected float distanciaEscapar = 15f;
    [SerializeField] protected bool autoseleccionarTarget = true;

    private void Start()
    {
        BuscarJugador();
    }

    private void LateUpdate()
    {
        if (target == null && autoseleccionarTarget && vivo)
        {
            BuscarJugador();
        }

        if (target != null)
        {
            distancia = Vector3.Distance(transform.position, target.position);
        }
        else
        {
            distancia = 999f;
        }

        CheckState();
    }

    private void BuscarJugador()
    {
        if (Personaje.singleton != null)
        {
            target = Personaje.singleton.transform;
        }
        else
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                target = jugador.transform;
            }
        }
    }

    private void CheckState()
    {
        if (target == null && state != States.muerto)
        {
            state = States.idle;
        }

        switch (state)
        {
            case States.idle:
                IdleState();
                break;
            case States.atacar:
                AttackState();
                break;
            case States.muerto:
                vivo = false;
                break;
            case States.seguir:
                FollowState();
                break;
        }
    }

    public void ChangeState(States e)
    {
        state = e;
    }

    public virtual void IdleState()
    {
        if (target != null && distancia < distanceFollow)
        {
            ChangeState(States.seguir);
        }
    }

    public virtual void AttackState()
    {
        if (target != null && distancia > distanciaAtacar + 0.4f)
        {
            ChangeState(States.seguir);
        }
    }

    public virtual void FollowState()
    {
        if (target == null) return;

        if (distancia < distanciaAtacar)
        {
            ChangeState(States.atacar);
        }
        else if (distancia > distanciaEscapar)
        {
            ChangeState(States.idle);
        }
    }

    public virtual void DeathState()
    {
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (this == null) return;

        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanceFollow);
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanciaAtacar);
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanciaEscapar);
    }
#endif

    private void OnDrawGizmos()
    {
        if (this == null || transform == null) return;

        int icono = (int)state;
        icono++;

        Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "0" + icono + ".png");
    }
}

public enum States
{
    idle = 0,
    seguir = 1,
    atacar = 2,
    muerto = 3
}
