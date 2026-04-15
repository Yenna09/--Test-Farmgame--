using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ControladorCicloDia : MonoBehaviour
{
    [SerializeField] private Light luzGlobal;
    [SerializeField] private CicloDia[] cicloDias;
    [SerializeField] private float tiempoPorCiclo;
    
    private float tiempoActualCiclo = 0;
    private float porcentajeCiclo;
    private int cicloActual = 0;
    private int cicloSiguiente = 1;

    private void Start() 
    {
        // Inicializamos con los valores del primer ciclo
        if (cicloDias.Length > 0)
        {
            AplicarCambios(cicloDias[0].colorCiclo, cicloDias[0].intensidad);
        }
    }

    private void Update() 
    {
        if (cicloDias.Length == 0) return;

        tiempoActualCiclo += Time.deltaTime;
        porcentajeCiclo = tiempoActualCiclo / tiempoPorCiclo;

        if (tiempoActualCiclo >= tiempoPorCiclo)
        {
            tiempoActualCiclo = 0;
            cicloActual = cicloSiguiente;

            if (cicloSiguiente + 1 > cicloDias.Length - 1)
                cicloSiguiente = 0;
            else
                cicloSiguiente += 1;
        }

        
        Color colorLerp = Color.Lerp(cicloDias[cicloActual].colorCiclo, cicloDias[cicloSiguiente].colorCiclo, porcentajeCiclo);
        
        
        float intensidadLerp = Mathf.Lerp(cicloDias[cicloActual].intensidad, cicloDias[cicloSiguiente].intensidad, porcentajeCiclo);

        AplicarCambios(colorLerp, intensidadLerp);
    }


    private void AplicarCambios(Color color, float intensidad)
{
    
    luzGlobal.color = color;
    luzGlobal.intensity = intensidad;
    RenderSettings.ambientLight = color * 0.5f; 

    
    GameObject player = GameObject.FindWithTag("Player");
    if (player != null)
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            
            sr.color = Color.white;
            sr.material.SetColor("_EmissionColor", Color.white);
            sr.material.SetColor("_Color", Color.white);
            sr.material.SetFloat("_Exposure", 1.0f);

            
            
        }
    }
}
}
