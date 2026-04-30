using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       
    public float smoothTime = 0.3f; 
    public Vector3 offset;         

    private Vector3 velocity = Vector3.zero; 

    void Start()
    {
        /*if (offset == Vector3.zero)
        {
            offset = transform.position - target.position;
        }*/
        
        // Al empezar, saltamos directo al target para evitar el deslizamiento inicial
        InstantSync();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        
        // Movimiento suave
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
    public void InstantSync()
    {
        if (target == null) return;
        
        // Posicionamos la cámara sin suavizado
        transform.position = target.position + offset;
        
        // Reseteamos la velocidad para que el SmoothDamp no tenga "inercia" vieja
        velocity = Vector3.zero;
    }
}
