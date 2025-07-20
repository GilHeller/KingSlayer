using UnityEngine;
using System;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    [Header("Audio Settings")]
    public AudioClip[] ouchSounds;
    private AudioSource audioSource;

    [Header("Animation Settings")]
    public Animator enemyAnimator; 
    public string takeDamageAnimationTrigger = "TakeDamage";
    public string deathAnimationTrigger = "Die";
    public float deathAnimationDuration = 10.0f;

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
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<Animator>();
            if (enemyAnimator == null)
            {
                Debug.LogWarning("Animator component not found on " + gameObject.name + ". Damage and death animations will not play.", this);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount); // Clamp health
        Debug.Log($"{gameObject.name} took {amount} damage. Current Health: {currentHealth}");

        // Play random ouch sound
        if (ouchSounds != null && ouchSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = UnityEngine.Random.Range(0, ouchSounds.Length);
            audioSource.PlayOneShot(ouchSounds[randomIndex]);
        }
        else if (ouchSounds != null && ouchSounds.Length == 0)
        {
            Debug.LogWarning("Ouch Sounds array is empty on " + gameObject.name + ". No sound will play.", this);
        }
        else if (ouchSounds == null)
        {
            Debug.LogWarning("Ouch Sounds array is not assigned on " + gameObject.name + ". No sound will play.", this);
        }

        // Trigger damage animation
        if (enemyAnimator != null && !string.IsNullOrEmpty(takeDamageAnimationTrigger))
        {
            enemyAnimator.SetTrigger(takeDamageAnimationTrigger);
        }
        else if (enemyAnimator == null)
        {
            Debug.LogWarning("Animator is not assigned or found on " + gameObject.name + ". Cannot play damage animation.", this);
        }
        else if (string.IsNullOrEmpty(takeDamageAnimationTrigger))
        {
            Debug.LogWarning("Take Damage Animation Trigger name is empty on " + gameObject.name + ". Cannot play damage animation.", this);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} has died.");

        if (enemyAnimator != null && !string.IsNullOrEmpty(deathAnimationTrigger))
        {
            enemyAnimator.SetTrigger(deathAnimationTrigger);
            StartCoroutine(DestroyAfterAnimationCoroutine());
        }
        else
        {
            Destroy(gameObject); // Fallback
        }
    }

    private IEnumerator DestroyAfterAnimationCoroutine()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
}
