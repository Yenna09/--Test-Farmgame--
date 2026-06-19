using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Ajustes de Ejes")]
    public float offsetY = -30f; 
    public float offsetZ = 0f;
    public Material lightMaterial;
    public Color dayColor;
    public Color nightColor;

    private float lerpValue;
    

    private float anguloObjetivoX;

    private void OnEnable()
    {

        DayNightManager.AlCambiarTiempo += CalcularAnguloSol;
    }

    private void OnDisable()
    {

        DayNightManager.AlCambiarTiempo -= CalcularAnguloSol;
    }

    private void CalcularAnguloSol(int hora, int minuto)
    {
        float tiempoEnHoras = hora + (minuto / 60f);
        
        anguloObjetivoX = (tiempoEnHoras - 6f) * 15f;

        

        //Debug.Log($"[Reloj del Juego] Avanzaron 10 minutos. Hora exacta: {hora:00}:{minuto:00}");
        transform.rotation = Quaternion.Euler(anguloObjetivoX, offsetY, offsetZ);

        
        if (tiempoEnHoras > 0 && tiempoEnHoras < 12)
        {
            lightMaterial.color = Color.Lerp(dayColor, nightColor, (tiempoEnHoras-12f)/12f*(-1));
        }

        if (tiempoEnHoras > 12 && tiempoEnHoras < 24)
        {
            lightMaterial.color = Color.Lerp(dayColor, nightColor, (tiempoEnHoras-12f) / 12f);
        }




    }

    
}