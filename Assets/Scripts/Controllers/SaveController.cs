using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public static SaveController Instance { get; private set; } 
    
    private string saveLocation;
    
    
    public List<string> destroyedItemsIDs = new List<string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            inventorySaveData = new InventorySaveData(),
            destroyedItemsIDs = this.destroyedItemsIDs // Guardamos la lista negra
        };

        SaveContainer(Inventory.Instance.hotbarContainer, saveData.inventorySaveData.savedItems, true);
        SaveContainer(Inventory.Instance.slotsContainer, saveData.inventorySaveData.savedItems, false);

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log("Juego guardado en: " + saveLocation);
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            
            // Cargamos la lista negra a la memoria de forma segura
            if (saveData.destroyedItemsIDs != null)
            {
                this.destroyedItemsIDs = saveData.destroyedItemsIDs;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = saveData.playerPosition;

                CameraFollow camScript = Camera.main.GetComponent<CameraFollow>();
                if (camScript != null) camScript.InstantSync();
            }

            if (saveData.inventorySaveData != null)
            {
                ClearContainer(Inventory.Instance.hotbarContainer);
                ClearContainer(Inventory.Instance.slotsContainer);

                foreach (SavedSlot savedItem in saveData.inventorySaveData.savedItems)
                {
                    Transform targetContainer = savedItem.isHotbar ? Inventory.Instance.hotbarContainer : Inventory.Instance.slotsContainer;
                    
                    if (savedItem.slotIndex < targetContainer.childCount)
                    {
                        Transform targetSlot = targetContainer.GetChild(savedItem.slotIndex);
                        Inventory.Instance.SpawnItemForSave(savedItem.itemID, savedItem.quantity, targetSlot);
                    }
                }
            }

            // --- LIMPIEZA DE ITEMS EN EL SUELO ---
            // Buscamos todas las manzanas/items que hay en el mundo 3D
            BaseItem[] itemsEnElSuelo = FindObjectsOfType<BaseItem>();
            foreach (BaseItem item in itemsEnElSuelo)
            {
                // Si el DNI de este ítem está en la lista negra que acabamos de cargar...
                if (this.destroyedItemsIDs.Contains(item.uniqueWorldID))
                {
                    Destroy(item.gameObject); // ...lo destruimos.
                }
            }
        }
        else
        {
            SaveGame();
        }
    }

    private void SaveContainer(Transform container, List<SavedSlot> list, bool isHotbar)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Transform slot = container.GetChild(i);
            if (slot.childCount > 0) 
            {
                ItemUI itemUI = slot.GetChild(0).GetComponent<ItemUI>();
                if (itemUI != null)
                {
                    list.Add(new SavedSlot
                    {
                        slotIndex = i,
                        itemID = itemUI.id,
                        quantity = itemUI.quantity,
                        isHotbar = isHotbar
                    });
                }
            }
        }
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform slot in container)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
    }
}