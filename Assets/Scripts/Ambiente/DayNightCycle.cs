using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Ajustes de Ejes")]
    public float offsetY = -30f; 
    public float offsetZ = 0f;

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

        Debug.Log($"[Reloj del Juego] Avanzaron 10 minutos. Hora exacta: {hora:00}:{minuto:00}");
        
        //Debug.Log($"[Sol] El nuevo ángulo objetivo en X es: {anguloObjetivoX}°");
    }

    void Update()
    {
        Quaternion rotacionDeseada = Quaternion.Euler(anguloObjetivoX, offsetY, offsetZ);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotacionDeseada, Time.deltaTime * 0.5f);
    }
}