using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Canvas quitMenu;
    public Button startText;
    public Button exitText;
    public Button yesButton;
    public Button noButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        quitMenu.enabled = false;
        Time.timeScale = 1f;
    }
    public void ExitPress()
    {
        quitMenu.enabled = true;
        startText.enabled = false;
        exitText.enabled = false;
    }

    public void NoPress()
    {
        quitMenu.enabled=false; 
        startText.enabled=true;
        exitText.enabled=true;
    }

    public void StartLevel()
    {
        Debug.Log("StartLevel");
        GameManager.Instance.gameData.health = 100;
        SceneManager.LoadScene("Play");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
