using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Scene To Load")]
    public string sceneName;

    [Header("Tag Settings")]
    public string triggeringTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggeringTag))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
