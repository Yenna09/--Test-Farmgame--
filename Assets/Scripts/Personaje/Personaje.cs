using System.Collections;
using UnityEngine;

public class Personaje : MonoBehaviour
{
    public static Personaje singleton;
    public Vida vida;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
    }
}
