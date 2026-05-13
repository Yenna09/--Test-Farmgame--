using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event footstepsEvent;
    [SerializeField]
    private AK.Wwise.Event hoeSFXEvent;
    [SerializeField]
    private AK.Wwise.Event wateringSFXEvent;
    public void PlayFootstep()
    {
        footstepsEvent.Post(gameObject);
    }
    
    public void UseHoe()
    {
        hoeSFXEvent.Post(gameObject);
    }

    public void WateringSFX()
    {
        Debug.Log("Regué");
        wateringSFXEvent.Post(gameObject);
    }



}
