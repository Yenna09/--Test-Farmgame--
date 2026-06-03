using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

// Esto nos permite configurar cada semilla desde el Inspector
[System.Serializable]
public class DatosSemilla
{
    public int idSemilla;
    public Sprite[] etapasCrecimiento; // Arrastrá los sprites (Semilla, Brote, Planta)
    public int idItemCosecha;
}

// Esta clase guarda la información de cada planta en la tierra
public class CultivoActivo
{
    public int idSemilla;
    public int diasCrecimiento;
    public GameObject objetoVisual; // El objeto 3D/2.5D en la escena
}

public class CropController : MonoBehaviour
{
    public static CropController Instance { get; private set; }

    [Header("Configuración de Cultivos")]
    public List<DatosSemilla> catalogoSemillas; 
    public GameObject prefabPlantaBase; // Un prefab vacío con un SpriteRenderer

    // Diccionario: Posición en la grilla -> Datos de la Planta
    private Dictionary<Vector3Int, CultivoActivo> cultivosActivos = new Dictionary<Vector3Int, CultivoActivo>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        // En lugar de AlAmanecer, escuchamos cada vez que el reloj hace "tic"
        DayNightManager.AlCambiarTiempo += ComprobarHoraCrecimiento;
    }

    private void OnDisable()
    {
        DayNightManager.AlCambiarTiempo -= ComprobarHoraCrecimiento;
    }

    private void ComprobarHoraCrecimiento(int hora, int minuto)
    {
        // Si son exactamente las 7:00 AM, hacemos crecer toda la granja
        if (hora == 7 && minuto == 0)
        {
            AvanzarDia();
            Debug.Log("🌱 [GRANJA] Son las 7:00 AM. ¡Las plantas acaban de crecer!");
        }
    }

    public void PlantarSemilla(Vector3Int posicion, int idSemillaPlantada)
    {
        if (!cultivosActivos.ContainsKey(posicion))
        {
            // 1. Calculamos el centro de la celda pero en el mundo 3D
            Vector3 posicionMundo = FarmingController.Instance.groundTilemap.GetCellCenterWorld(posicion);
            
            // 2. Instanciamos el Prefab verticalmente (Sin rotación acotada)
            GameObject nuevaPlanta = Instantiate(prefabPlantaBase, posicionMundo, Quaternion.identity);
            
            // 3. Registramos la planta en nuestro sistema
            CultivoActivo nuevoCultivo = new CultivoActivo
            {
                idSemilla = idSemillaPlantada,
                diasCrecimiento = 0,
                objetoVisual = nuevaPlanta
            };

            cultivosActivos.Add(posicion, nuevoCultivo);
            
            // 4. Actualizamos el dibujito
            ActualizarVisualCultivo(nuevoCultivo);
        }
    }

    public void AvanzarDia()
    {
        List<Vector3Int> posiciones = new List<Vector3Int>(cultivosActivos.Keys);

        foreach (Vector3Int pos in posiciones)
        {
            TileBase pisoActual = FarmingController.Instance.groundTilemap.GetTile(pos);
            
            if (pisoActual == FarmingController.Instance.tierraMojadaTile)
            {
                cultivosActivos[pos].diasCrecimiento++; 
                FarmingController.Instance.groundTilemap.SetTile(pos, FarmingController.Instance.tierraAradaTile);
            }

            ActualizarVisualCultivo(cultivosActivos[pos]);
        }
    }

    void ActualizarVisualCultivo(CultivoActivo cultivo)
    {
        // Buscamos los sprites correspondientes a esta semilla en el catálogo
        DatosSemilla datos = catalogoSemillas.Find(s => s.idSemilla == cultivo.idSemilla);
        
        if (datos != null && datos.etapasCrecimiento.Length > 0)
        {
            SpriteRenderer sr = cultivo.objetoVisual.GetComponent<SpriteRenderer>();
            
            // Nos aseguramos de no pasarnos del límite del array si la planta ya creció al máximo
            int etapa = Mathf.Min(cultivo.diasCrecimiento, datos.etapasCrecimiento.Length - 1);
            
            sr.sprite = datos.etapasCrecimiento[etapa];
        }
    }

    // Función extra por si querés comprobar si una celda está ocupada
    public bool HayCultivoEn(Vector3Int posicion)
    {
        return cultivosActivos.ContainsKey(posicion);
    }
    public bool EsCosechable(Vector3Int posicion)
    {
        if (cultivosActivos.TryGetValue(posicion, out CultivoActivo cultivo))
        {
            DatosSemilla datos = catalogoSemillas.Find(s => s.idSemilla == cultivo.idSemilla);
            if (datos != null)
            {
                // Es cosechable si los días que pasaron alcanzan o superan la cantidad de sprites que tiene
                return cultivo.diasCrecimiento >= datos.etapasCrecimiento.Length - 1;
            }
        }
        return false;
    }

    public void Cosechar(Vector3Int posicion)
    {
        if (cultivosActivos.TryGetValue(posicion, out CultivoActivo cultivo))
        {
            DatosSemilla datos = catalogoSemillas.Find(s => s.idSemilla == cultivo.idSemilla);

            if (datos != null)
            {
                // 1. Usamos TU función PickUpItem directamente
                bool recogidoExitosamente = Inventory.Instance.PickUpItem(datos.idItemCosecha, 1);

                // 2. Si entró en la mochila (devolvió true), destruimos la planta
                if (recogidoExitosamente)
                {
                    Destroy(cultivo.objetoVisual);
                    cultivosActivos.Remove(posicion);
                    
                    // Volvemos la tierra a estado arado normal
                    FarmingController.Instance.groundTilemap.SetTile(posicion, FarmingController.Instance.tierraAradaTile);
                    
                    Debug.Log("[COSECHA] ¡Cultivo guardado en la mochila!");
                }
                else
                {
                    // Si devolvió false, la planta se queda ahí esperando a que hagas espacio
                    Debug.Log("[COSECHA] La planta no se cosechó porque no hay espacio en el inventario.");
                }
            }
        }
    }
    public List<DatosCultivoGuardado> ExportarCultivos()
    {
        List<DatosCultivoGuardado> lista = new List<DatosCultivoGuardado>();
        foreach (var kvp in cultivosActivos)
        {
            lista.Add(new DatosCultivoGuardado {
                posicion = kvp.Key,
                idSemilla = kvp.Value.idSemilla,
                diasCrecimiento = kvp.Value.diasCrecimiento
            });
        }
        return lista;
    }

    public void ImportarCultivos(List<DatosCultivoGuardado> lista)
    {
        if (lista == null) return;
        cultivosActivos.Clear();
        
        foreach (var guardado in lista)
        {
            // Instanciamos el cultivo igual que en PlantarSemilla
            Vector3 posicionMundo = FarmingController.Instance.groundTilemap.GetCellCenterWorld(guardado.posicion);
            GameObject nuevaPlanta = Instantiate(prefabPlantaBase, posicionMundo, Quaternion.identity);

            CultivoActivo nuevoCultivo = new CultivoActivo
            {
                idSemilla = guardado.idSemilla,
                diasCrecimiento = guardado.diasCrecimiento,
                objetoVisual = nuevaPlanta
            };

            cultivosActivos.Add(guardado.posicion, nuevoCultivo);
            ActualizarVisualCultivo(nuevoCultivo);
        }
    }
}