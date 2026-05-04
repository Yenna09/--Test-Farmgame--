using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    public static HotbarController Instance { get; private set; }

    [Header("Configuración Visual")]
    public Color selectedColor = new Color(1f, 1f, 0.5f, 1f); // Amarillo suave para el slot activo
    public Color normalColor = Color.white; // Color normal del slot

    [Header("Estado")]
    public int selectedSlotIndex = 0; // Guardamos qué slot está activo (0 a 7)

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Por defecto, al arrancar el juego seleccionamos el primer slot
        SelectSlot(0);
    }

    void Update()
    {
        // 1. Detectamos las teclas numéricas del 1 al 9
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SelectSlot(8);

        // 2. Detectamos la ruedita del mouse
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            Transform hotbar = Inventory.Instance.hotbarContainer;
            if (hotbar != null && hotbar.childCount > 0)
            {
                int maxSlots = hotbar.childCount;

                if (scroll > 0f) // Ruedita hacia arriba
                {
                    selectedSlotIndex++;
                    // Si nos pasamos del máximo, volvemos al slot 0
                    if (selectedSlotIndex >= maxSlots) 
                    {
                        selectedSlotIndex = 0;
                    }
                }
                else if (scroll < 0f) // Ruedita hacia abajo
                {
                    selectedSlotIndex--;
                    // Si bajamos de 0, vamos al último slot
                    if (selectedSlotIndex < 0) 
                    {
                        selectedSlotIndex = maxSlots - 1;
                    }
                }

                // Llamamos a la función para que actualice colores y equipe el ítem
                SelectSlot(selectedSlotIndex);
            }
        }
    }

    public void SelectSlot(int index)
    {
        Transform hotbar = Inventory.Instance.hotbarContainer;

        if (hotbar == null) return;

        // 1. Restaurar el color normal de todos los slots
        for (int i = 0; i < hotbar.childCount; i++)
        {
            Image slotImage = hotbar.GetChild(i).GetComponent<Image>();
            if (slotImage != null) slotImage.color = normalColor;
        }

        // 2. Pintar el slot seleccionado para dar feedback visual
        selectedSlotIndex = index;
        if (index >= 0 && index < hotbar.childCount)
        {
            Image selectedImage = hotbar.GetChild(index).GetComponent<Image>();
            if (selectedImage != null) selectedImage.color = selectedColor;
        }

        // 3. Revisar qué ítem hay adentro y "equiparlo"
        CheckEquippedItem();
    }

    // Esta función revisa el slot activo y nos dice qué tenemos en la mano
    public void CheckEquippedItem()
    {
        Transform hotbar = Inventory.Instance.hotbarContainer;

        // Nos aseguramos de que el índice sea válido
        if (selectedSlotIndex < 0 || selectedSlotIndex >= hotbar.childCount) return;

        Transform selectedSlot = hotbar.GetChild(selectedSlotIndex);

        // Si el slot tiene un "hijo", significa que hay un ítem adentro
        if (selectedSlot.childCount > 0)
        {
            ItemUI item = selectedSlot.GetChild(0).GetComponent<ItemUI>();
            if (item != null)
            {
                Debug.Log($"[EQUIPADO] Tenés en la mano el Ítem ID: {item.id} (Cantidad: {item.quantity})");
                
                // FUTURO: Acá llamarías a tu script de animaciones o combate.
                // Ej: PlayerEquipment.EquipModel(item.id);
            }
        }
        else
        {
            Debug.Log("[EQUIPADO] Manos vacías.");
            
            // FUTURO: Acá ocultarías el modelo 3D de la mano.
        }
    }

    // NUEVO: Devuelve el ID del ítem equipado, o -1 si la mano está vacía
    public int GetEquippedItemID()
    {
        Transform hotbar = Inventory.Instance.hotbarContainer;
        if (selectedSlotIndex < 0 || selectedSlotIndex >= hotbar.childCount) return -1;

        Transform selectedSlot = hotbar.GetChild(selectedSlotIndex);
        if (selectedSlot.childCount > 0)
        {
            ItemUI item = selectedSlot.GetChild(0).GetComponent<ItemUI>();
            if (item != null) return item.id;
        }
        return -1; // Mano vacía
    }
}