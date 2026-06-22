using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnTimeChangeAudio : MonoBehaviour
{
    [SerializeField]
    [Header("Pongan el DayNightManager acá.")]
    private DayNightManager dayNightManager;

    string sceneName;
    UnityEngine.SceneManagement.Scene m_Scene;

    private void Start()
    {
        m_Scene = SceneManager.GetActiveScene();
        sceneName = m_Scene.name;
    }


    private void OnEnable()
    {
        DayNightManager.AlAmanecer += setDayState;
        DayNightManager.AlAnochecer += setNightState;
    }
    private void OnDisable()
    {
        DayNightManager.AlAmanecer -= setDayState;
        DayNightManager.AlAnochecer -= setNightState;
    }
   
    private void setDayState()
    {
        Debug.Log("ESTADO DIA");
        AkUnitySoundEngine.SetState("Time", "Day");
        if (sceneName == "PrototipoMain") AkUnitySoundEngine.PostEvent("Play_DayNightStart", this.gameObject);

    }
    private void setNightState()
    {
        Debug.Log("ESTADO NOCHE");
        AkUnitySoundEngine.SetState("Time", "Night");
        if (sceneName == "PrototipoMain") AkUnitySoundEngine.PostEvent("Play_DayNightStart", this.gameObject);
    }

}
