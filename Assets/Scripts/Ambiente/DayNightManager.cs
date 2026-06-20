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
    // Transmite dos números: la hora y el minuto
    public static event Action<int, int> AlCambiarTiempo; 

    public static bool esDeDia;
    private float temporizador = 0f;

    // Creamos la instancia global (como hicimos con el SaveController)
    public static DayNightManager Instance { get; private set; }

    void Awake()
    {
        //Siempre me asigno como la instancia oficial al cargar la escena
        Instance = this;
    }
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

        // Avisamos a cualquier script que necesite saber la hora exacta
        AlCambiarTiempo?.Invoke(horaActual, minutoActual);

        ComprobarCicloDiaNoche();
    }

    private void ComprobarCicloDiaNoche()
    {
        // Si es de noche, y llegamos a la hora y minuto exacto del amanecer
        if (!esDeDia && horaActual == horaAmanecer && minutoActual >= 0)
        {
            esDeDia = true;
            AlAmanecer?.Invoke();
            Debug.Log("Buenos dias! a laburaaaar");
        }
        // Si es de día, y llegamos a la hora y minuto exacto del anochecer
        else if (esDeDia && horaActual == horaAnochecer && minutoActual >= 0)
        {
            esDeDia = false;
            AlAnochecer?.Invoke();
            Debug.Log("Es de noche. A mimir😴");
        }
    }
    public void DormirHasta(int horaDestino, bool nuevoDia)
    {
        horaActual = horaDestino;
        minutoActual = 0;
        temporizador = 0f; // Reseteamos el contador de tiempo real para que no tire un tick inmediatamente

        if (nuevoDia)
        {
            esDeDia = true;
            AlAmanecer?.Invoke(); //Esto hace que el CropController haga crecer las plantas
            Debug.Log("[SUEÑO] Amaneció un nuevo día. ¡A laburar!");
        }
        else
        {
            esDeDia = false;
            AlAnochecer?.Invoke(); // Avisa al juego que se hizo de noche
            Debug.Log("[SUEÑO] Te despertaste por la noche.");
        }

        // Actualizamos a todos los que escuchen la hora (el sol, la UI del reloj, etc.)
        AlCambiarTiempo?.Invoke(horaActual, minutoActual);
    }
    public void ForzarSincronizacion()
    {
        esDeDia = (horaActual >= horaAmanecer && horaActual < horaAnochecer);
        
        // Dispara los eventos para cualquier otro script que escuche
        AlCambiarTiempo?.Invoke(horaActual, minutoActual);
        
        // Te lo muestra directo en la consola para que te quedes tranquilo
        Debug.Log($"[TIEMPO SINCRONIZADO] Acabás de cargar la escena. Son las {horaActual:00}:{minutoActual:00}");
    }
}