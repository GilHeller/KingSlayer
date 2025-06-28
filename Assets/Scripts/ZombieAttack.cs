using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public int damageAmount = 10; // Amount of damage the zombie deals

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Assuming the player has a PlayerHealth script that manages health
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Apply damage to the player
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
