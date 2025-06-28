using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public CameraShake cameraShake;
    public AudioSource audioSource;  // Reference to AudioSource component
    public AudioClip damageSound;
    public Text healthText;
    public AudioClip gameOverSound;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player Health: " + health);
        UpdateHealthText();

        if (cameraShake != null)
        {
            cameraShake.Shake();
        }
        
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Check if the player is `
        if (health <= 0)
        {
            Die();
        }
    }


    public void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + health;
        }
    }

    private void Die()
    {
        // Handle player death
        Debug.Log("Player is dead!");

        audioSource.PlayOneShot(gameOverSound);
        GameData.isGameOver = true;
        SceneManager.LoadScene("Menu");
    }
}
