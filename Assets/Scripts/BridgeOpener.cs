using UnityEngine;

public class BridgeOpener : MonoBehaviour
{
    public Transform[] bridgePivots;
    public float openAngle = -90f;
    public float rotationSpeed = 30f;
    private bool shouldOpen = false;

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
        }
    }

    public void TriggerOpen()
    {
        shouldOpen = true;
    }
}
