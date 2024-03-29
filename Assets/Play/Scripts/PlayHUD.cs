using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI heightTxt;
    public TextMeshProUGUI comboTxt;

    public Button resumeBtn;
    public Button quitBtn;
    public Button retryBtn;

    public GameObject pausePanel;

    private void Awake()
    {
        comboTxt.text = "";
        scoreTxt.text = "Score: 0";
        heightTxt.text = "Height: 0m";

        resumeBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
        retryBtn.onClick.RemoveAllListeners();

        resumeBtn.onClick.AddListener(ResumeClick);
        quitBtn.onClick.AddListener(QuitClick);
        retryBtn.onClick.AddListener(RetryClick);
    }

    private void Update()
    {
        pausePanel.SetActive(GameManager.Instance.isGamePaused);
    }


    private void FixedUpdate()
    {
        scoreTxt.text = $"Score: {GameManager.Instance.Floor * 10}";
        heightTxt.text = $"Floor: {GameManager.Instance.Floor}";

        int comboCounter = GameManager.Instance.ComboCounter;

        HandleComboCounterText(comboCounter);
    }

    private void HandleComboCounterText(int comboCounter)
    {
        if (comboCounter <= 0)
        {
            comboTxt.text = "";
        }
        else
        {
            comboTxt.text = $"Combo: {comboCounter}";
        }
    }

    private void ResumeClick()
    {
        GameManager.Instance.UnpauseGame();
    }

    private void RetryClick()
    {
        GameManager.Instance.RestartGame();
    }

    private void QuitClick()
    {
        SceneManager.LoadScene("Title");
    }
}
