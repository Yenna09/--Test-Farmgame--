using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Database db;

    [SerializeField]
    private GameObject Text;

    public int id;
    public int quantity;

    [HideInInspector]
    public Database.InventoryItem itemData;
    [HideInInspector]
    public Transform exParent;

    public TextMeshProUGUI quantityText;
    Image iconoImage;
    Vector3 dragOffset;

    void Awake()
    {

        iconoImage = GetComponent<Image>();
        
        // Solo buscamos el texto si no lo asignaste a mano en el Inspector
        if (quantityText == null) 
        {
            quantityText = GetComponentInChildren<TextMeshProUGUI>();
        }

        exParent = transform.parent;
        iconoImage = GetComponent<Image>();

        exParent = transform.parent;
        if (exParent.GetComponent<Image>())
        {
            exParent.GetComponent<Image>().fillCenter = true;
        }

        InitializeItem(id, quantity);
    }
    
    public void RefreshUI()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }

        // Si la cantidad es 0 o menos, el objeto no debería existir en la UI
        if (quantity <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void InitializeItem(int _id, int _quantity)
    {
        this.id = _id;
        this.quantity = _quantity;

        // Accedemos directamente al array de la Database usando el ID
        itemData = db.dataBase[id];

        // Configuramos lo visual
        iconoImage.sprite = itemData.icono;
        
        RefreshUI();
    }
    // En ItemUI.cs
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Solo dividimos si hay más de 1 ítem
            if (quantity > 1)
            {
                int splitAmount = quantity / 2; // Divide la pila a la mitad
                quantity -= splitAmount;
                RefreshUI();

                // Usamos PickUpItem para que el inventario busque automáticamente 
                // el primer slot vacío (Hotbar primero, luego Inv)
                Inventory.Instance.PickUpItem(id, splitAmount); 
            }
        }
    }
}