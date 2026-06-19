using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class VidaPlayer : MonoBehaviour
{
    [SerializeField] private float saludMaxima = 3f;
    private float saludActual;

    [Header("Knockback e Invulnerabilidad")]
    [SerializeField] private float tiempoInvulnerabilidad = 3.5f; 
    private float tiempoUltimoGolpe;    

    private bool estaSiendoEmpujado = false;
    private float tiempoRestanteEmpuje;
    private Vector3 direccionEmpuje;
    private float fuerzaEmpuje;
    private Rigidbody rb;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        saludActual = saludMaxima;
    }

    public void RecibirDamageConKnockback(float damage, Vector3 direccion, float fuerza, float duracion)
    {
        // Si todavía estamos en el tiempo de invulnerabilidad, ignoramos el ataque
        if (Time.time < tiempoUltimoGolpe + tiempoInvulnerabilidad)
        {
            return;
        }

        // Registramos en qué momento exacto nos pegaron
        tiempoUltimoGolpe = Time.time;

        CausarDamage(damage);

        estaSiendoEmpujado = true;
        tiempoRestanteEmpuje = duracion;
        direccionEmpuje = direccion;
        fuerzaEmpuje = fuerza;
    }

    private void Update()
    {
        if (estaSiendoEmpujado)
        {
            tiempoRestanteEmpuje -= Time.deltaTime;

            if (tiempoRestanteEmpuje <= 0)
            {
                estaSiendoEmpujado = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (estaSiendoEmpujado)
        {
            Vector3 nuevaPosicion = rb.position + direccionEmpuje * fuerzaEmpuje * Time.fixedDeltaTime;
            rb.MovePosition(nuevaPosicion);
        }
    }

    public void CausarDamage(float d)
    {
        saludActual -= d;
        Debug.Log($"¡Au! Salud actual: {saludActual}"); // Agregamos un log para ver cómo baja la vida

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("¡El jugador ha muerto!");
        gameObject.SetActive(false);
    }
}