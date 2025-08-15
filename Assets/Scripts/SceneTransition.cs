using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;
    public Image fadeImage;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadSceneWithFade());
        }
    }

    public void OnExitButtonClick()
    {
        StartCoroutine(LoadSceneWithFade());
    }

    private IEnumerator LoadSceneWithFade()
    {
        // Start fade out animation
        if (fadeImage != null)
            fadeImage.GetComponent<Animator>().SetBool("FadeOut", true);

        yield return new WaitForSeconds(1f); // Wait for fade to finish

        // Save before switching
        if (GameManager.Instance != null)
            GameManager.Instance.SaveGameData();

        // Subscribe to scene loaded callback
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load new scene
        SceneManager.LoadScene(targetScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe to avoid double calls
        SceneManager.sceneLoaded -= OnSceneLoaded;

        Debug.Log("Scene Loaded: " + scene.name);

        // Assign player after scene is fully loaded
        if (GameManager.Instance != null && !GameManager.Instance.gameData.activePlayer)
        {
            Debug.Log("___ INSIDE IF");
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                Debug.Log("BEFORE: " + GameManager.Instance.gameData.activePlayer);
                GameManager.Instance.gameData.activePlayer = playerObj;
                Debug.Log("AFTER: " + GameManager.Instance.gameData.activePlayer);

                // Reactivate to reset any state
                playerObj.SetActive(false);
                playerObj.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No GameObject with tag 'Player' found in the new scene!");
            }
        }

        // Mark scene as new for game logic
        if (GameManager.Instance != null)
            GameManager.Instance.gameData.isNewScene = true;

        // Optionally fade in after load
        if (fadeImage != null)
            fadeImage.GetComponent<Animator>().SetBool("FadeIn", true);
    }
}
