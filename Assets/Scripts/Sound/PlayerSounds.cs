using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event footstepsEvent;
    public void PlayFootstep()
    {
        footstepsEvent.Post(gameObject);
    }
}
