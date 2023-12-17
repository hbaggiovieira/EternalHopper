using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleHUD : MonoBehaviour
{
    public Button closeBtn;
    public Button playBtn;
    public Button highscoresBtn;
    public Button creditsBtn;

    private void Start()
    {
        closeBtn.onClick.RemoveAllListeners();
        playBtn.onClick.RemoveAllListeners();
        highscoresBtn.onClick.RemoveAllListeners();
        creditsBtn.onClick.RemoveAllListeners();

        playBtn.onClick.AddListener(PlayClick);
        highscoresBtn.onClick.AddListener(HighscoresClick);
        closeBtn.onClick.AddListener(CloseClick);
        creditsBtn.onClick.AddListener(CreditsClick);
    }

    private void CloseClick()
    {
        GameManager.Instance.QuitGame();
    }

    private void PlayClick()
    {
        SceneManager.LoadScene("Play");
    }

    private void HighscoresClick()
    {
        SceneManager.LoadScene("Highscores");
    }

    private void CreditsClick()
    {
        SceneManager.LoadScene("Credits");
    }
}