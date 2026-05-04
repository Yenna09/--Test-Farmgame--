using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Database db;

    [SerializeField]
    private GameObject Text; // Este no lo estás usando, pero te lo dejo para no romperte el Inspector

    public int id;
    public int quantity;

    [HideInInspector]
    public Database.InventoryItem itemData;
    [HideInInspector]
    public Transform exParent;

    public TextMeshProUGUI quantityText;
    private Image iconoImage;

    void Awake()
    {
        iconoImage = GetComponent<Image>();
        
        if (quantityText == null) 
        {
            quantityText = GetComponentInChildren<TextMeshProUGUI>();
        }

        exParent = transform.parent;

        // EL FIX: Primero chequeamos que el padre exista (!= null) antes de pedirle la Image
        if (exParent != null && exParent.GetComponent<Image>() != null)
        {
            exParent.GetComponent<Image>().fillCenter = true;
        }

        // Borré el InitializeItem() de acá. No hace falta llamarlo en el Awake
        // porque tu script de Inventory ya lo llama con los datos correctos justo después de crearlo.
    }
    
    public void RefreshUI()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }

        // Si la cantidad es 0 o menos, el objeto se destruye
        if (quantity <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void InitializeItem(int _id, int _quantity)
    {
        this.id = _id;
        this.quantity = _quantity;

        // Red de seguridad: si no asignaste la db en el prefab, usa la del Inventario
        if (db == null && Inventory.Instance != null)
        {
            db = Inventory.Instance.db;
        }

        // Accedemos a la base de datos
        itemData = db.dataBase[id];

        // Configuramos lo visual
        if (iconoImage != null)
        {
            iconoImage.sprite = itemData.icono;
        }
        
        RefreshUI();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (quantity > 1)
            {
                int splitAmount = quantity / 2;
                quantity -= splitAmount;
                RefreshUI();

                Inventory.Instance.PickUpItem(id, splitAmount); 
            }
        }
    }
}