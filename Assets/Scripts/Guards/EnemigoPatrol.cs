using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemigoPatrol : Enemy
{
    #region Graph Data
    [SerializeField] private List<Vector3> nodosDelGrafo = new List<Vector3>();
    private int indiceNodoActual;
    private Vector3 destinoActual;
    #endregion

    #region Movement Settings
    [SerializeField] private float velocidadMovimiento = 3f;
    [SerializeField] private float distanciaCambioNodo = 0.5f;
    [SerializeField] private float distanciaFrenado = 1.5f; 
    private float distanciaCambioNodoSqr;
    private Rigidbody rb;
    #endregion

    #region Combat Settings
    [SerializeField] private float damage = 1f;
    [SerializeField] public float velocidadAtaque = 1.5f;
    [SerializeField] private float tiempoEsperaRetorno = 1f;
    
    [Header("Knockback Settings")]
    [SerializeField] private float fuerzaKnockback = 10f; 
    [SerializeField] private float duracionKnockback = 0.2f; 

    [Header("Stun / Recovery Settings")]
    [SerializeField] private float tiempoEsperaPostAtaque = 4f; 

    private float tiempoRetorno;
    private float tiempoSiguienteAtaque;
    private float tiempoFinRecuperacion;
    private bool estaEnRecuperacion = false;
    #endregion

    #region References
    private SpriteRenderer spriteRenderer;
    #endregion

    public void ConstruirGrafo(Transform[] rutasDelSpawner)
    {
        nodosDelGrafo.Clear();
        if (rutasDelSpawner == null || rutasDelSpawner.Length == 0) return;

        for (int i = 0; i < rutasDelSpawner.Length; i++)
        {
            nodosDelGrafo.Add(rutasDelSpawner[i].position);
        }

        destinoActual = nodosDelGrafo[0];
    }

    public void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody>(); 
        distanciaCambioNodoSqr = distanciaCambioNodo * distanciaCambioNodo;
    }

    #region State Machine Overrides
    public override void IdleState()
    {
        base.IdleState();

        if (target != null)
        {
            tiempoRetorno = 0f;
        }
        else
        {
            if (tiempoRetorno <= 0f) tiempoRetorno = Time.time + tiempoEsperaRetorno;
            if (Time.time < tiempoRetorno) return;
        }

        if (nodosDelGrafo.Count == 0) return;

        if (target == null)
        {
            destinoActual = ObtenerPuntoMasCercanoEnGrafo();
        }
        else
        {
            destinoActual = nodosDelGrafo[indiceNodoActual];
        }

        MoverHaciaDestino(destinoActual, 0.1f);

        if ((destinoActual - transform.position).sqrMagnitude < distanciaCambioNodoSqr)
        {
            indiceNodoActual = (indiceNodoActual + 1) % nodosDelGrafo.Count;
        }
    }

    public override void FollowState()
    {
        if (estaEnRecuperacion)
        {
            if (Time.time >= tiempoFinRecuperacion)
            {
                estaEnRecuperacion = false;
            }
            return;
        }

        base.FollowState();

        if (target != null)
        {
            MoverHaciaDestino(target.position, distanciaFrenado);
        }
    }

    public override void AttackState()
    {
        if (estaEnRecuperacion)
        {
            if (Time.time >= tiempoFinRecuperacion)
            {
                estaEnRecuperacion = false;
            }
            return;
        }

        base.AttackState();

        if (target != null)
        {
            MoverHaciaDestino(target.position, distanciaFrenado);
        }

        if (Time.time >= tiempoSiguienteAtaque)
        {
            Atacar();
            
            estaEnRecuperacion = true;
            tiempoFinRecuperacion = Time.time + tiempoEsperaPostAtaque;
            tiempoSiguienteAtaque = tiempoFinRecuperacion + velocidadAtaque;
        }
    }

    public override void DeathState()
    {
        base.IdleState();
    }
    #endregion

    #region Combat Logic
    public void Atacar()
    {
        if (Personaje.singleton != null && Personaje.singleton.vida != null && target != null)
        {
            Vector3 diferenciaPlana = target.position - transform.position;
            diferenciaPlana.y = 0;
            Vector3 direccionEmpuje = diferenciaPlana.normalized;

            Personaje.singleton.vida.RecibirDamageConKnockback(damage, direccionEmpuje, fuerzaKnockback, duracionKnockback);
        }
    }
    #endregion

    #region Movement Logic
    private void MoverHaciaDestino(Vector3 destino, float limiteAcercamiento)
    {
        Vector3 destinoPlano = new Vector3(destino.x, transform.position.y, destino.z);
        float distanciaAlDestino = Vector3.Distance(transform.position, destinoPlano);

        if (distanciaAlDestino <= limiteAcercamiento)
        {
            return; 
        }

        float direccionX = destinoPlano.x - transform.position.x;
        
        Vector3 nuevaPosicion = Vector3.MoveTowards(transform.position, destinoPlano, velocidadMovimiento * Time.deltaTime);
        rb.MovePosition(nuevaPosicion);

        ActualizarSprite(direccionX);
    }

    private void ActualizarSprite(float velocidadX)
    {
        if (spriteRenderer == null) return;

        if (velocidadX > 0.05f)
        {
            spriteRenderer.flipX = false;
        }
        else if (velocidadX < -0.05f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private Vector3 ObtenerPuntoMasCercanoEnGrafo()
    {
        if (nodosDelGrafo.Count == 0) return transform.position;
        if (nodosDelGrafo.Count == 1) return nodosDelGrafo[0];

        Vector3 puntoMasCercano = nodosDelGrafo[0];
        float distanciaMinimaSqr = float.MaxValue;

        for (int i = 0; i < nodosDelGrafo.Count; i++)
        {
            Vector3 nodoA = nodosDelGrafo[i];
            Vector3 nodoB = nodosDelGrafo[(i + 1) % nodosDelGrafo.Count];

            Vector3 proyeccion = ProyectarPuntoEnSegmento(transform.position, nodoA, nodoB);
            float distSqr = (transform.position - proyeccion).sqrMagnitude;

            if (distSqr < distanciaMinimaSqr)
            {
                distanciaMinimaSqr = distSqr;
                puntoMasCercano = proyeccion;
                indiceNodoActual = (i + 1) % nodosDelGrafo.Count;
            }
        }

        return puntoMasCercano;
    }

    private Vector3 ProyectarPuntoEnSegmento(Vector3 punto, Vector3 inicioArista, Vector3 finArista)
    {
        Vector3 direccionArista = finArista - inicioArista;
        float longitudAristaSqr = direccionArista.sqrMagnitude;

        if (longitudAristaSqr == 0f) return inicioArista;

        float t = Vector3.Dot(punto - inicioArista, direccionArista) / longitudAristaSqr;
        t = Mathf.Clamp01(t);

        return inicioArista + t * direccionArista;
    }
    #endregion
}