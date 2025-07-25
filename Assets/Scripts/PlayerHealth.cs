using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    [Header("UI Settings")]
    public Text healthText;

    [Header("Audio Settings")]
    public AudioClip[] ouchSounds;
    private AudioSource audioSource;

    [Header("Animation Settings")]
    public Animator playerAnimator;
    public string takeDamageAnimationTrigger = "TakeDamage";
    public string deathAnimationTrigger = "Die";
    public float deathAnimationDuration = 5.0f;

    private bool isDead = false;

    void Awake()
    {
        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.maxDistance = 20f;
        audioSource.volume = 0.7f;

        // Setup Animator
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogWarning("Animator component not found on " + gameObject.name + ". Animations will not play.", this);
            }
        }

        // Initialize health UI
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Debug.Log($"{gameObject.name} took {amount} damage. Current Health: {currentHealth}");

        GameManager.Instance.gameData.health = (int)currentHealth;    

        UpdateHealthUI();

        // Play damage sound
        if (ouchSounds != null && ouchSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, ouchSounds.Length);
            audioSource.PlayOneShot(ouchSounds[randomIndex]);
        }

        // Trigger damage animation
        if (playerAnimator != null && !string.IsNullOrEmpty(takeDamageAnimationTrigger))
        {
            playerAnimator.SetTrigger(takeDamageAnimationTrigger);
        }

        // Check death
        if (GameManager.Instance.gameData.health <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null && GameManager.Instance != null && GameManager.Instance.gameData != null)
        {
            healthText.text = "Health: " + GameManager.Instance.gameData.health.ToString();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} has died.");

        if (playerAnimator != null && !string.IsNullOrEmpty(deathAnimationTrigger))
        {
            playerAnimator.SetTrigger(deathAnimationTrigger);
            StartCoroutine(HandleDeathAfterAnimation());
        }
        else
        {
            // Fallback
            // Destroy(gameObject);
            gameObject.SetActive(false); // Disable instead of destroy  
            SceneManager.LoadScene("PlayerLossMenu");
        }
    }

    private IEnumerator HandleDeathAfterAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        // Destroy(gameObject);
        gameObject.SetActive(false); // Disable instead of destroy
        SceneManager.LoadScene("PlayerLossMenu");
    }
}
