using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public Button startButton;
    public Button backToMenuButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => StartGame());
        }
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.AddListener(() => ReturnToMenu());
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }
    void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
