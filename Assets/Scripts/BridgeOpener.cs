using UnityEngine;

public class BridgeOpener : MonoBehaviour
{
    public Transform[] bridgePivots;
    public float openAngle = -90f;
    public float rotationSpeed = 30f;
    private bool shouldOpen = false;
    private bool hasPlayedSound = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;

    private void Update()
    {
        if (shouldOpen)
        {
            foreach (Transform pivot in bridgePivots)
            {
                Quaternion targetRotation = Quaternion.Euler(openAngle, 0, 0);
                pivot.localRotation = Quaternion.RotateTowards(
                    pivot.localRotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            // Play the sound once when opening starts
            if (!hasPlayedSound && audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
                hasPlayedSound = true;
            }
        }
    }

    public void TriggerOpen()
    {
        shouldOpen = true;
    }
}
