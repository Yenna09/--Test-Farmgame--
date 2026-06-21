using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class VidaPlayer : MonoBehaviour
{
    [SerializeField] private float saludMaxima = 3f;
    private float saludActual;

    [Header("Knockback e Invulnerabilidad")]
    [SerializeField] private float tiempoInvulnerabilidad = 0.5f; 
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
        if (Time.time < tiempoUltimoGolpe + tiempoInvulnerabilidad)
        {
            return;
        }

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

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        gameObject.SetActive(false);
    }
}
