using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    public float shakeAmount = 0.2f; // How much the camera will shake
    public float shakeDuration = 0.5f; // Duration of the shake effect

    private void Start()
    {
        originalPosition = transform.localPosition; // Save the initial position of the camera
    }

    // Call this method to trigger the shake
    public void Shake()
    {
        StopAllCoroutines(); // Stop any ongoing shake
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Reset to the original position after shaking
        transform.localPosition = originalPosition;
    }
}
