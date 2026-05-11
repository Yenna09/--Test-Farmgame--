using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event footstepsEvent;
    [SerializeField]
    private AK.Wwise.Event hoeSFXEvent;
    public void PlayFootstep()
    {
        footstepsEvent.Post(gameObject);
    }
    
    public void UseHoe()
    {
        hoeSFXEvent.Post(gameObject);
    }
}
