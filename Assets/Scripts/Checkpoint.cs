using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]

    public Transform spawnPoint;
    
    [Tooltip("Player GameObject to teleport")]
    private GameObject player;

    private CharacterController characterController;
    public Image fadeImage;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached checkpoint: " + gameObject.name);
            StartCoroutine(TeleportPlayerHere());
        }
    }

    public IEnumerator TeleportPlayerHere()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned on checkpoint: " + gameObject.name);
            yield break;
        }

        if (characterController != null) characterController.enabled = false;

        player.transform.position = spawnPoint.position;

        if (characterController != null) characterController.enabled = true;

        fadeImage.GetComponent<Animator>().SetBool("FadeIn", true);
        // fadeImage.GetComponent<Animator>().SetBool("FadeOut", true);

        yield return new WaitForSeconds(1f);
        fadeImage.GetComponent<Animator>().SetBool("FadeIn", false);
    }
}
