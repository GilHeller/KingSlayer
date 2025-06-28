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
    
    private IEnumerator LoadSceneWithFade()
    {
        fadeImage.GetComponent<Animator>().SetBool("fade", true);
        // yield return new WaitUntil(() => fadeImage.color.a == 1);
        yield return new WaitForSeconds(2f);
        LoadScene();
    }
    public void LoadScene()
    {
        // if (GameManager.Instance != null)
        // {
        //     GameManager.Instance.SaveGameData();
        // }
        Debug.Log("Loading scene: " + targetScene);
        
        SceneManager.LoadScene(targetScene);
    }
}