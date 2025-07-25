using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;
    public Image fadeImage;
    // public Transform playerSpawnPoint;
    
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
        fadeImage.GetComponent<Animator>().SetBool("FadeOut", true);
        // yield return new WaitUntil(() => fadeImage.color.a == 1);
        yield return new WaitForSeconds(1f);
        LoadScene();
    }
    public void LoadScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGameData();
        }
        Debug.Log("Loading scene: " + targetScene);


        SceneManager.LoadScene(targetScene);

        if (!GameManager.Instance.gameData.activePlayer)
            GameManager.Instance.gameData.activePlayer = GameObject.FindGameObjectWithTag("Player");

        GameManager.Instance.gameData.isNewScene = true;
        GameManager.Instance.gameData.activePlayer.SetActive(false);
        GameManager.Instance.gameData.activePlayer.SetActive(true);
        fadeImage.GetComponent<Animator>().SetBool("FadeIn", true);
    }
}