using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private bool playerInRange = false;
    private GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Add weapon to player's inventory
            player.GetComponent<PlayerInventory>().hasWeapon = true;
            // Destroy the weapon pickup object
            Destroy(gameObject);
        }
    }
}
