using UnityEngine;

public class DoorSound : MonoBehaviour
{
    public AudioSource doorAudio;

    public void PlayDoorSound()
    {
        if (doorAudio != null && !doorAudio.isPlaying)
        {
            doorAudio.Play();
        }
    }
}
