using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverHUD : MonoBehaviour
{
    public Button tryAgainBtn;
    public Button highscoresBtn;
    public TextMeshProUGUI textScore;

    void Start()
    {

        if (GameManager.Instance != null)
        {
            textScore.text = $"Score: {GameManager.Instance.Floor * 10}";
        }

        tryAgainBtn.onClick.RemoveAllListeners();
        tryAgainBtn.onClick.AddListener(TryAgainClick);

        highscoresBtn.onClick.RemoveAllListeners();
        highscoresBtn.onClick.AddListener(HighscoresClick);

    }

    private void TryAgainClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Floor = 0;
        }

        SceneManager.LoadScene("Play");
    }

    private void HighscoresClick()
    {
        SceneManager.LoadScene("Highscores");
    }
}
