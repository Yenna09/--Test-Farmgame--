using UnityEngine;
using System;

public class DayNightManager : MonoBehaviour
{
    [Header("Configuración del Tiempo")]
    [Tooltip("A qué hora de la mañana amanece (ej: 6)")]
    public int horaAmanecer = 6;
    [Tooltip("A qué hora de la tarde/noche anochece (ej: 20 para las 8PM)")]
    public int horaAnochecer = 20;
    
    [Tooltip("Cuántos segundos reales tardan en pasar 10 minutos en el juego")]
    public float segundosRealesPorTick = 1.5f; 

    [Header("Tiempo Actual")]
    [Range(0, 23)] public int horaActual = 6;
    [Range(0, 50)] public int minutoActual = 0;

    // Eventos clásicos
    public static event Action AlAmanecer;
    public static event Action AlAnochecer;
    
    // ¡NUEVO EVENTO! Útil para actualizar la UI del reloj o cambiar la música
    // Transmite dos números: la hora y el minuto
    public static event Action<int, int> AlCambiarTiempo; 

    private bool esDeDia = true;
    private float temporizador = 0f;

    void Start()
    {
        // Determinamos el estado inicial en base a la hora de inicio
        esDeDia = (horaActual >= horaAmanecer && horaActual < horaAnochecer);
    }

    void Update()
    {
        // Acumulamos el tiempo real que pasa
        temporizador += Time.deltaTime;

        // Si el tiempo acumulado supera nuestro límite, avanzamos el reloj del juego
        if (temporizador >= segundosRealesPorTick)
        {
            temporizador -= segundosRealesPorTick; // Reseteamos el temporizador
            AvanzarTiempo();
        }
    }

    private void AvanzarTiempo()
    {
        // Avanzamos de 10 en 10
        minutoActual += 10;

        if (minutoActual >= 60)
        {
            minutoActual = 0;
            horaActual++;

            if (horaActual >= 24)
            {
                horaActual = 0; // Medianoche, nuevo día
            }
        }

        // Avisamos a cualquier script que necesite saber la hora exacta (UI, Música)
        AlCambiarTiempo?.Invoke(horaActual, minutoActual);

        ComprobarCicloDiaNoche();
    }

    private void ComprobarCicloDiaNoche()
    {
        // Si es de noche, y llegamos a la hora y minuto exacto del amanecer
        if (!esDeDia && horaActual == horaAmanecer && minutoActual == 0)
        {
            esDeDia = true;
            AlAmanecer?.Invoke();
            Debug.Log("Buenos dias! a laburaaaar");
        }
        // Si es de día, y llegamos a la hora y minuto exacto del anochecer
        else if (esDeDia && horaActual == horaAnochecer && minutoActual == 0)
        {
            esDeDia = false;
            AlAnochecer?.Invoke();
            Debug.Log("Es de noche. A mimir😴");
        }
    }
}