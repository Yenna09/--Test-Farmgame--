using System;
using UnityEngine;

public class OnTimeChangeAudio : MonoBehaviour
{
    [SerializeField]
    [Header("Pongan el DayNightManager acá.")]
    private DayNightManager dayNightManager;

  
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
        AkUnitySoundEngine.SetState("Time", "Day");
        AkUnitySoundEngine.PostEvent("Play_DayNightStart", this.gameObject);
    }
    private void setNightState()
    {
        AkUnitySoundEngine.SetState("Time", "Night");
        AkUnitySoundEngine.PostEvent("Play_DayNightStart", this.gameObject);
    }

}
