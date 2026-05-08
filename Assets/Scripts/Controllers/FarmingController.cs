using UnityEngine;
using UnityEngine.Tilemaps;

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
    // NUEVO: Agregamos el tile mojado
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
        if (playerTransform == null || playerAnimator == null) return;

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

        // Chequeamos qué tiene en la mano
        int idEnMano = HotbarController.Instance.GetEquippedItemID();
        TileBase tileActual = groundTilemap.GetTile(celdaObjetivo);

        bool mostrarCursor = false;

        // Lógica para Azada -> Solo en Pasto
        if (idEnMano == playerMovement.idDeLaAzada && tileActual == pastoTile)
        {
            mostrarCursor = true;
        }
        // Lógica para Regadera -> Solo en Tierra Arada Seca
        else if (idEnMano == playerMovement.idDeLaRegadera && tileActual == tierraAradaTile)
        {
            mostrarCursor = true;
        }

        cursorObjeto.gameObject.SetActive(mostrarCursor);
    }

    // El PlayerMovement llamará a esta función
    public void UsarHerramienta()
    {
        int idEnMano = HotbarController.Instance.GetEquippedItemID();
        TileBase tileActual = groundTilemap.GetTile(celdaObjetivo);

        // Si es la azada y hay pasto -> Aramas
        if (idEnMano == playerMovement.idDeLaAzada && tileActual == pastoTile)
        {
            groundTilemap.SetTile(celdaObjetivo, tierraAradaTile);
        }
        // Si es la regadera y hay tierra seca -> Mojamos
        else if (idEnMano == playerMovement.idDeLaRegadera && tileActual == tierraAradaTile)
        {
            groundTilemap.SetTile(celdaObjetivo, tierraMojadaTile);
            Debug.Log("[CULTIVO] ¡Tierra regada!");
        }
    }
}