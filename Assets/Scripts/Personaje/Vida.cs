using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Vida : MonoBehaviour
{
    public float vidaInicial;
    public float vidaActual;

    public UnityEvent eventoMorir;
    void Start()
    {
        vidaActual = vidaInicial;
    }

    // Update is called once per frame
    public void CausarDamage(float cuanto)
    {
        vidaActual -= cuanto;
        if (vidaActual <= 0)
        {
            Debug.Log("Esta muerto" + gameObject.name);
            eventoMorir.Invoke();
        }
    }
}
