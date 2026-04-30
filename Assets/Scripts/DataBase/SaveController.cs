using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        LoadGame();
    }

    // Update is called once per frame
    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // 1. Movemos al jugador a la posición guardada
                player.transform.position = saveData.playerPosition;

                // 2. Buscamos el script de la cámara y le pedimos que salte al jugador
                CameraFollow camScript = Camera.main.GetComponent<CameraFollow>();
                if (camScript != null)
                {
                    camScript.InstantSync();
                }
            }
        }
        else
        {
            SaveGame();
        }
    }
}
