using UnityEngine;
using UnityEngine.UI;

public class ChangeVolumeLevels : MonoBehaviour
{
    public Slider thisSlider;
    public float masterVolume;
    public float musicVolume;
    public float SFXVolume;

    public void SetSpecificValue(string whatValue)
    {
        
        if (whatValue == "Master")
        {
            masterVolume = thisSlider.value;
            AkUnitySoundEngine.SetRTPCValue("MasterVolume", masterVolume);
             
        }

        if (whatValue == "Sounds")
        {
            SFXVolume = thisSlider.value;
            AkUnitySoundEngine.SetRTPCValue("SFXVolume", SFXVolume);

        }

        if (whatValue == "Music")
        {
            musicVolume = thisSlider.value;
            AkUnitySoundEngine.SetRTPCValue("MusicVolume", musicVolume);

        }
    }
}
