using UnityEngine;

public class BreakOnPlayerHit : MonoBehaviour
{
    [Header("Fractured Version Prefab")]
    public GameObject fracturedPrefab; // Drag your cube_voronoi prefab here

    [Header("Settings")]
    public string playerTag = "Player"; // Tag your player GameObject as "Player"
    public float explosionForce = 300f;
    public float explosionRadius = 2f;

    private bool isBroken = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        // Check if the player hit the cube
        if (collision.gameObject.CompareTag(playerTag))
        {
            BreakCube();
        }
    }

    void BreakCube()
    {
        isBroken = true;

        // Instantiate fractured version at the same location and rotation
        GameObject broken = Instantiate(fracturedPrefab, transform.position, transform.rotation);

        // Apply physics explosion to each piece
        foreach (Rigidbody rb in broken.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.5f, ForceMode.Impulse);
        }

        // Remove the original cube
        Destroy(gameObject);
    }
}
