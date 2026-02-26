using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultivoManager : MonoBehaviour
{
    public static CultivoManager Instance;

    

    private void Awake()
    {
        Instance = this;
    }
    public Dictionary<int, DatosCultivos> diccionarioCultivos = new Dictionary<int, DatosCultivos>();

    public List<int> inventarioIDs = new List<int>();

    void Start()
    {
        CargarDatos();
    }

    void CargarDatos() 
    {
        TextAsset archivo = Resources.Load<TextAsset>("CultivosData");
        ContenedorCultivos contenedor = JsonUtility.FromJson<ContenedorCultivos>(archivo.text);

        foreach (DatosCultivos item in contenedor.listaCultivos) 
        {
            diccionarioCultivos.Add(item.id, item);
        }
        
        Debug.Log("El cultivo con ID 1 es: " + diccionarioCultivos[1].nombre);

        //Check de seguridad
        if (diccionarioCultivos != null && diccionarioCultivos.Count > 0)
        {
            Debug.Log("<color=green> BASE DE DATOS OK:</color> Se han cargado " + diccionarioCultivos.Count + " cultivos.");
        
        
            foreach(var kvp in diccionarioCultivos)
            {
                Debug.Log($"<color=cyan>Test de Carga:</color> ID {kvp.Key} detectado como {kvp.Value.nombre}");
                break; 
            }
        }   
        else
        {
            Debug.LogError("<color=red> ERROR EN BASE DE DATOS:</color> El diccionario está vacío. Revisa el JSON o la ruta.");
        }
    }

}
