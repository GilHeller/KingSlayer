using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    private int health;
    public CameraShake cameraShake;
    public AudioSource audioSource;  // Reference to AudioSource component
    public AudioClip damageSound;
    public Text healthText;
    public AudioClip gameOverSound;
    
    void Start()
    {
        int health = GameManager.Instance.gameData.health; // Initialize health from GameData
        
        // Initialize audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (GameManager.Instance.gameData.currentEquippedWeapon.weaponType == WeaponType.Shield)
        {
            // If the player has a shield equipped, reduce damage
            damage = damage += GameManager.Instance.gameData.currentEquippedWeapon.damage;
            Debug.Log("Shield is active, damage reduced to: " + damage);
        }   
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
