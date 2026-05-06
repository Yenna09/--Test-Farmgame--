using UnityEngine;

public class Detector : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public LayerMask obstructionMask;

    [Header("Settings")]
    public float sphereRadius = 0.5f; 
    public Vector3 targetOffset = new Vector3(0, 1.0f, 0); 

    private FadeObstacle _currentObstacle;

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        // --- LÓGICA INVERTIDA ---
        // El origen ahora es el personaje (donde nace el rayo)
        Vector3 originPos = transform.position + targetOffset;
        
        // El destino es la posición de la cámara
        Vector3 targetPos = cameraTransform.position;

        // La dirección apunta hacia la cámara
        Vector3 direction = (targetPos - originPos).normalized;
        
        // La distancia máxima es el espacio entre ambos
        float distance = Vector3.Distance(originPos, targetPos);

        // Disparamos desde el personaje hacia la cámara
        if (Physics.SphereCast(originPos, sphereRadius, direction, out RaycastHit hit, distance, obstructionMask))
        {
            FadeObstacle fade = hit.collider.GetComponent<FadeObstacle>();

            if (fade != null)
            {
                if (fade != _currentObstacle)
                {
                    if (_currentObstacle != null)
                        _currentObstacle.FadeIn();

                    _currentObstacle = fade;
                    _currentObstacle.FadeOut();
                }
                return; 
            }
        }

        // Si no golpea nada, restauramos el obstáculo anterior
        if (_currentObstacle != null)
        {
            _currentObstacle.FadeIn();
            _currentObstacle = null;
        }
    }

    // Gizmos invertidos para que veas el rayo saliendo del conejo
    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;

        Gizmos.color = Color.cyan; // Cambiamos a cian para diferenciar esta versión
        
        Vector3 originPos = transform.position + targetOffset;
        Vector3 targetPos = cameraTransform.position;

        // Dibujamos la esfera de origen en el personaje
        Gizmos.DrawWireSphere(originPos, sphereRadius);
        // Dibujamos la línea hacia la cámara
        Gizmos.DrawLine(originPos, targetPos);
        // Dibujamos la esfera final en la cámara
        Gizmos.DrawWireSphere(targetPos, sphereRadius);
    }
}