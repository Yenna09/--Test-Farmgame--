using System.Collections;
using UnityEngine;

public class FadeObstacle : MonoBehaviour
{
    [Header("Fade Settings")]
    [Range(0f, 1f)] public float fadedAlpha = 0.2f;
    public float fadeDuration = 0.25f;

    private Renderer _renderer;
    private Material _material;
    private float _originalAlpha;
    private Coroutine _fadeRoutine;
    
    // Variable crítica para evitar que la corrutina se reinicie infinitamente
    // y para que el estado se mantenga "eterno" mientras se cumpla la condición.
    private float _currentTargetAlpha; 

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError($"¡No se encontró Renderer en {gameObject.name}!");
            return;
        }

        // Creamos una instancia única del material para este objeto.
        // Esto evita que todos los objetos de la escena se vuelvan transparentes a la vez.
        _material = _renderer.material; 

        Color c = _material.GetColor(BaseColorID);
        _originalAlpha = c.a;
        
        // Inicializamos el target con el alpha original para estar en estado "Opaco"
        _currentTargetAlpha = _originalAlpha;
    }

    public void FadeOut()
    {
        float target = _originalAlpha * fadedAlpha;
        StartFade(target);
    }

    public void FadeIn()
    {
        StartFade(_originalAlpha);
    }

    private void StartFade(float targetAlpha)
    {
        // Si ya estamos intentando llegar a ese alpha (o ya estamos ahí), ignoramos la orden.
        // Esto soluciona el parpadeo de 1 segundo porque no reinicia la corrutina cada frame.
        if (Mathf.Approximately(_currentTargetAlpha, targetAlpha))
            return;

        _currentTargetAlpha = targetAlpha;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeCoroutine(targetAlpha));
    }

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        Color c = _material.GetColor(BaseColorID);
        float startAlpha = c.a;
        float t = 0f;

        // Si la duración es casi cero, aplicamos el cambio de inmediato.
        if (fadeDuration <= 0.01f)
        {
            c.a = targetAlpha;
            _material.SetColor(BaseColorID, c);
            _fadeRoutine = null;
            yield break;
        }

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            
            // Interpolación lineal suave entre el alpha actual y el objetivo
            float a = Mathf.Lerp(startAlpha, targetAlpha, t);
            
            c.a = a;
            _material.SetColor(BaseColorID, c);
            yield return null;
        }

        // Aseguramos el valor final exacto al terminar para evitar errores de redondeo
        c.a = targetAlpha;
        _material.SetColor(BaseColorID, c);
        
        _fadeRoutine = null;
    }

    // Limpieza al destruir el objeto para evitar fugas de memoria (Memory Leaks)
    // Ya que '_renderer.material' crea una instancia que Unity no destruye sola.
    private void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
        }
    }
}