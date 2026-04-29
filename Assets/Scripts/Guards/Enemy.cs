using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : MonoBehaviour
{
    public States state;
    [SerializeField] public float distanceFollow;
    [SerializeField] public float distanciaAtacar;
    [SerializeField] public float distanciaEscapar;

    public bool autoseleccionarTarget = true;
    public Transform target;
    public float distancia; 

    public bool vivo = true;

    public void Awake() 
    {
        if (autoseleccionarTarget) 
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
        
        
        StartCoroutine(CalcularDistancia());
    }

    private void LateUpdate() 
    {
        CheckState();
    }

    private void CheckState()
    {
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

    // Máquina de estados (Virtuales para que puedas heredarlos)
    public virtual void IdleState()
    {
        if (distancia < distanceFollow)
        {
            ChangeState(States.seguir);
        }
    }
    public virtual void AttackState()
    {
        if (distancia > distanciaAtacar + 0.4f)
        {
            ChangeState(States.seguir);
        }
    }
    public virtual void FollowState()
    {
        if (distancia < distanciaAtacar)
        {
            ChangeState(States.atacar);
        }
        else if(distancia > distanciaEscapar)
        {
            ChangeState(States.idle);
        }
    }
    public virtual void DeathState() 
    { 
        
    }

    IEnumerator CalcularDistancia()
    {
        while(vivo)
        {
            
            if (target != null)
            {
                distancia = Vector3.Distance(transform.position, target.position);
            }

            yield return new WaitForSeconds(0.3f); 
        }
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

        int icono = (int) state;
        icono++;
        
        Gizmos.DrawIcon(transform.position + Vector3.up * 1.2f, "0" + icono + ".png");
    }
}

public enum States
{
    idle = 0,
    seguir = 1,
    atacar = 2,
    muerto = 3
}