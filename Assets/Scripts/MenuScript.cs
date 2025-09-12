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
        quitMenu.enabled = false;

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
        SceneManager.LoadScene("Play");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
