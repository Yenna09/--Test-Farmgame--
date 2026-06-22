using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemigoPatrol : Enemy
{
    #region Graph Data
    [SerializeField] private List<Vector3> nodosDelGrafo = new List<Vector3>();
    private int indiceNodoActual;
    private Vector3 destinoActual;
    #endregion

    #region Movement Speeds
    [Header("Movement Speeds")]
    [SerializeField] private float velocidadPatrullaje = 3f;
    [SerializeField] private float velocidadPersecucion = 8f;
    private float velocidadActual;

    [SerializeField] private float distanciaCambioNodo = 0.5f;
    [SerializeField] private float distanciaFrenado = 1.3f; 
    private float distanciaCambioNodoSqr;
    private Rigidbody rb;
    #endregion

    #region Combat Settings
    [SerializeField] private float damage = 1f;
    [SerializeField] private float velocidadAtaque = 1.5f;
    [SerializeField] private float tiempoEsperaRetorno = 1f;
    
    [Header("Knockback Settings")]
    [SerializeField] private float fuerzaKnockback = 10f; 
    [SerializeField] private float duracionKnockback = 0.15f; 

    [Header("Stun / Recovery Settings")]
    [SerializeField] private float tiempoEsperaPostAtaque = 1.6f; 
    [SerializeField] private float tiempoAnticipacion = 0.3f; 
    [SerializeField] private Color colorAnticipacion = new Color(1f, 0.5f, 0.5f); 

    private float tiempoSiguienteAtaque;
    private bool estaEnRecuperacion = false;
    private bool estaAtacando = false;
    private Color colorOriginal;
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

        destinoActual = ObtenerPuntoMasCercanoEnGrafo();
    }

    public void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) colorOriginal = spriteRenderer.color;
        
        rb = GetComponent<Rigidbody>(); 
        distanciaCambioNodoSqr = distanciaCambioNodo * distanciaCambioNodo;
        velocidadActual = velocidadPatrullaje;
    }

    #region State Machine Overrides
    public override void IdleState()
    {
        velocidadActual = velocidadPatrullaje;
        base.IdleState(); // Esto chequea si el jugador entró en el rango de visión

        // Si base.IdleState nos pasó al estado de Seguir, abortamos el patrullaje
        if (state != States.idle) return;

        if (nodosDelGrafo.Count == 0) return;

        // Caminamos tranquilos hacia el siguiente nodo
        MoverHaciaDestino(destinoActual, 0.1f);

        // Si llegamos al nodo, pasamos al siguiente
        if ((destinoActual - transform.position).sqrMagnitude < distanciaCambioNodoSqr)
        {
            indiceNodoActual = (indiceNodoActual + 1) % nodosDelGrafo.Count;
            destinoActual = nodosDelGrafo[indiceNodoActual];
        }
    }

    public override void FollowState()
    {
        velocidadActual = velocidadPersecucion;

        if (estaEnRecuperacion || estaAtacando) return;

        base.FollowState(); // Esto chequea si el jugador logró alejarse

        // ¡Si el código base acaba de pasarnos a Idle, significa que perdimos el aggro!
        if (state == States.idle)
        {
            // Recalculamos el camino más corto para no volver cruzando todo el mapa
            destinoActual = ObtenerPuntoMasCercanoEnGrafo();
            return;
        }

        if (target != null)
        {
            MoverHaciaDestino(target.position, distanciaFrenado);
        }
    }

    public override void AttackState()
    {
        velocidadActual = velocidadPersecucion;

        if (estaEnRecuperacion || estaAtacando) return;

        base.AttackState();

        if (target != null)
        {
            MoverHaciaDestino(target.position, distanciaFrenado);
        }

        if (Time.time >= tiempoSiguienteAtaque)
        {
            StartCoroutine(RutinaAtacar());
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

    private IEnumerator RutinaAtacar()
    {
        estaAtacando = true;

        if (spriteRenderer != null) spriteRenderer.color = colorAnticipacion;
        yield return new WaitForSeconds(tiempoAnticipacion);
        if (spriteRenderer != null) spriteRenderer.color = colorOriginal;

        if (target != null && Vector3.Distance(transform.position, target.position) <= distanciaAtacar)
        {
            Atacar();
        }
        estaAtacando = false;

        estaEnRecuperacion = true;
        yield return new WaitForSeconds(tiempoEsperaPostAtaque);
        estaEnRecuperacion = false;

        tiempoSiguienteAtaque = Time.time + velocidadAtaque;
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
        
        Vector3 nuevaPosicion = Vector3.MoveTowards(transform.position, destinoPlano, velocidadActual * Time.deltaTime);
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