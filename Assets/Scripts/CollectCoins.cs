using UnityEngine;
using UnityEngine.UI;

public class CollectCoins : MonoBehaviour
{
    private int coinCount = 0;
    public Text coinText;
    public AudioClip coinSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coinCount;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinCount++;
            UpdateCoinUI();
            Debug.Log("Coin Collected!");

            Debug.Log("coinSound: " + coinSound);
            Debug.Log("audioSource: " + audioSource);
            if (coinSound != null && audioSource != null)
            {
                Debug.Log("Play");
                audioSource.PlayOneShot(coinSound);
            }

            Destroy(other.gameObject);
        }
    }
}
