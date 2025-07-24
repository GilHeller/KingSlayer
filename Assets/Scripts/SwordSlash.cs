using System.Collections;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    public int damage = 25;
    public KeyCode attackKey = KeyCode.Mouse0;
    private bool canSlash = true;
    public AudioClip slashSound;
    public AudioSource audioSource;


    void Update()
    {
        if (Input.GetKeyDown(attackKey) && canSlash)
        {
            Debug.Log("Swing!");
            canSlash = false;
            audioSource.PlayOneShot(slashSound);
            StartCoroutine(SwingSword());
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.Log("No audioSource - creating a new one");
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        Collider swordCol = GetComponent<Collider>();
        Collider playerCol = GameObject.FindWithTag("Player").GetComponent<Collider>();

        if (swordCol != null && playerCol != null)
        {
            Physics.IgnoreCollision(swordCol, playerCol);
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (Input.GetKeyDown(attackKey) && other.tag == "Zombie")
        {
            ZombieHealth zh = other.GetComponent<ZombieHealth>();
            if (zh != null)
            {
                zh.TakeDamage(damage);
            }
        }
    }
    IEnumerator SwingSword()
    {
        Vector3 originalPos = transform.localPosition;
        Vector3 targetPos = originalPos + transform.forward * 0.5f;  // move sword forward

        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(originalPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPos;

        // Return to original position
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(targetPos, originalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        canSlash = true; // allow next slash
    }

}

