using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FarmingController : MonoBehaviour
{
    public static FarmingController Instance { get; private set; }

    [Header("Referencias del Jugador")]
    public Transform playerTransform;
    public Animator playerAnimator;
    public PlayerMovement playerMovement;

    [Header("Referencias del Entorno")]
    public Tilemap groundTilemap; 
    public TileBase pastoTile;    
    public TileBase tierraAradaTile; 
    
    public TileBase tierraMojadaTile; 

    [Header("Visuales")]
    public Transform cursorObjeto; 

    private Vector3Int celdaObjetivo; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (cursorObjeto != null && tierraAradaTile is Tile tileTierra)
        {
            SpriteRenderer sr = cursorObjeto.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = tileTierra.sprite;
                sr.color = new Color(1f, 1f, 1f, 0.5f); 
            }
        }
    }

    void Update()
    {
        if (playerTransform == null || playerAnimator == null || groundTilemap == null) return;

        Vector3Int celdaDelJugador = groundTilemap.WorldToCell(playerTransform.position);

        int dirX = Mathf.RoundToInt(playerAnimator.GetFloat("LastHorizontal"));
        int dirZ = Mathf.RoundToInt(playerAnimator.GetFloat("LastVertical")); 

        if (Mathf.Abs(dirX) > 0 && Mathf.Abs(dirZ) > 0) dirZ = 0;
        else if (dirX == 0 && dirZ == 0) dirZ = -1;

        celdaObjetivo = celdaDelJugador;
        celdaObjetivo.x += dirX; 
        celdaObjetivo.y += dirZ; 
        celdaObjetivo.z = 0;     

        ActualizarCursor();
    }

    void ActualizarCursor()
    {
        if (cursorObjeto == null) return;

        Vector3 posicionCursor = groundTilemap.GetCellCenterWorld(celdaObjetivo);
        posicionCursor.y = groundTilemap.transform.position.y + 0.05f; 
        cursorObjeto.position = posicionCursor;
        cursorObjeto.rotation = Quaternion.Euler(90f, 0f, 0f); 

        int idEnMano = HotbarController.Instance.GetEquippedItemID();
        TileBase tileActual = groundTilemap.GetTile(celdaObjetivo);
        bool mostrarCursor = false;

        // Comprobamos que el ID sea válido dentro de la base de datos
        if (idEnMano >= 0 && idEnMano < Inventory.Instance.db.dataBase.Length)
        {
            // Obtenemos struct InventoryItem
            Database.InventoryItem itemEquipado = Inventory.Instance.db.dataBase[idEnMano];

            // Usamos enum ActionType 
            if (itemEquipado.accion == Database.ActionType.arar && tileActual == pastoTile)
            {
                mostrarCursor = true;
            }
            else if (itemEquipado.accion == Database.ActionType.regar && tileActual == tierraAradaTile)
            {
                mostrarCursor = true;
            }
            else if (itemEquipado.accion == Database.ActionType.plantar && (tileActual == tierraAradaTile || tileActual == tierraMojadaTile))
            {
                mostrarCursor = true;
            }
        }

        cursorObjeto.gameObject.SetActive(mostrarCursor);
    }

    public void UsarHerramienta()
    {
        int idEnMano = HotbarController.Instance.GetEquippedItemID();
        TileBase tileActual = groundTilemap.GetTile(celdaObjetivo);

        if (idEnMano < 0 || idEnMano >= Inventory.Instance.db.dataBase.Length) return;

        // Obtenemos struct InventoryItem
        Database.InventoryItem itemEquipado = Inventory.Instance.db.dataBase[idEnMano];

        if (itemEquipado.accion == Database.ActionType.arar && tileActual == pastoTile)
        {
            groundTilemap.SetTile(celdaObjetivo, tierraAradaTile);
        }
        else if (itemEquipado.accion == Database.ActionType.regar && tileActual == tierraAradaTile)
        {
            groundTilemap.SetTile(celdaObjetivo, tierraMojadaTile);
        }
        else if (itemEquipado.accion == Database.ActionType.plantar && (tileActual == tierraAradaTile || tileActual == tierraMojadaTile))
        {
            if (!CropController.Instance.HayCultivoEn(celdaObjetivo))
            {
                CropController.Instance.PlantarSemilla(celdaObjetivo, idEnMano);
                HotbarController.Instance.ConsumeItemEquipped();
            }
        }
    }
    public bool PuedeCosechar()
    {
        return CropController.Instance.HayCultivoEn(celdaObjetivo) && CropController.Instance.EsCosechable(celdaObjetivo);
    }

    public void EjecutarCosecha()
    {
        CropController.Instance.Cosechar(celdaObjetivo);
    }
    public List<DatosTerrenoGuardado> ExportarTerreno()
    {
        List<DatosTerrenoGuardado> lista = new List<DatosTerrenoGuardado>();
        
        if (groundTilemap == null) return lista;
        // Revisamos todo el mapa buscando tierra arada o mojada
        BoundsInt limites = groundTilemap.cellBounds;
        foreach (Vector3Int pos in limites.allPositionsWithin)
        {
            TileBase tileActual = groundTilemap.GetTile(pos);
            if (tileActual == tierraAradaTile)
                lista.Add(new DatosTerrenoGuardado { posicion = pos, estado = "arado" });
            else if (tileActual == tierraMojadaTile)
                lista.Add(new DatosTerrenoGuardado { posicion = pos, estado = "mojado" });
        }
        return lista;
    }

    public void ImportarTerreno(List<DatosTerrenoGuardado> lista)
    {
        if (lista == null || groundTilemap == null) return;
        foreach (var guardado in lista)
        {
            if (guardado.estado == "arado") groundTilemap.SetTile(guardado.posicion, tierraAradaTile);
            else if (guardado.estado == "mojado") groundTilemap.SetTile(guardado.posicion, tierraMojadaTile);
        }
    }
}