using UnityEngine;
using System; 

public class DayNightManager : MonoBehaviour
{
    [Header("Configuración del Sol")]
    [Tooltip("Arrastra aquí tu Directional Light")]
    public Transform sol; 
    
    
    public static event Action AlAmanecer;
    public static event Action AlAnochecer;

    // Estado interno para saber en qué momento del día estamos
    private bool esDeDia = true;

    void Update()
    {
        // La rotación en X del sol suele determinar la altura. 
        // Asumiendo que 0 a 180 grados es cuando el sol está sobre el horizonte:
        float anguloSol = sol.eulerAngles.x;
        
        // Ajusta estos valores si la rotación de tu luz es diferente
        bool esDeDiaAhora = anguloSol > 0f && anguloSol < 180f;

        // Si antes era de día, y ahora el cálculo dice que no, ¡se hizo de noche!
        if (esDeDia && !esDeDiaAhora)
        {
            esDeDia = false;
            // El operador ?. invoca el evento solo si hay alguien escuchando
            AlAnochecer?.Invoke(); 
            Debug.Log("Es de noche. A mimir😴");
        }
        // Si antes era de noche, y ahora el cálculo dice que sí, ¡amaneció!
        else if (!esDeDia && esDeDiaAhora)
        {
            esDeDia = true;
            AlAmanecer?.Invoke();
            Debug.Log("Buenos dias! a laburaaaar");
        }
    }
}