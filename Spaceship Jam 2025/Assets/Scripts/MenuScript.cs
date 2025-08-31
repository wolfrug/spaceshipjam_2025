using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public Button startButton;
    public Button backToMenuButton;

    public MusicType musicToPlayOnstart = MusicType.MUSIC_AMBIENT;
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
        AudioManager.instance.PlayMusic(musicToPlayOnstart);
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
