using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighscoresHUD : MonoBehaviour
{

    public TextMeshProUGUI[] highscoreTexts;
    public Button closeBtn;

    private void Start()
    {
        UpdateHighscores();

        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(CloseClick);
    }

    private void UpdateHighscores()
    {
        HighscoreList highscoreList = Highscores.LoadHighscores();

        for (int i = 0; i < highscoreTexts.Length; i++)
        {
            if (i < highscoreList.highscores.Count)
            {
                highscoreTexts[i].text = $"{i + 1}. {highscoreList.highscores[i]}";
            }
            else
            {
                highscoreTexts[i].text = $"{i + 1}. -------------";
            }
        }
    }

    private void CloseClick()
    {
        GameManager.Instance.Score = 0;
        SceneManager.LoadScene("Title");
    }
}