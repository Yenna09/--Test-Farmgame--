using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class AudioManager : MonoBehaviour
{
    string sceneName;
    Scene m_Scene;

    bool menuAudioisPlaying;
    private void Awake()
    {
         GameObject[] objs = GameObject.FindGameObjectsWithTag("Audio");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        
        m_Scene = SceneManager.GetActiveScene();
        sceneName = m_Scene.name;
        if (sceneName == "MainMenuScene" & menuAudioisPlaying == false)
        {
            AkUnitySoundEngine.PostEvent("Play_MainMenu", this.gameObject);
            menuAudioisPlaying = true;
        }
    }
    

    

}
