using UnityEngine;

public class Personaje : MonoBehaviour
{
    public static Personaje singleton; 
    public VidaPlayer vida; 

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject); 
        }
    }
}