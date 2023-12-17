using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt;
    public Button resumeBtn;
    public Button quitBtn;

    public GameObject pausePanel;

    private void Awake()
    {
        scoreTxt.text = "Score: 0";

        resumeBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();

        resumeBtn.onClick.AddListener(ResumeClick);
        quitBtn.onClick.AddListener(QuitClick);
    }

    private void Update()
    {
        pausePanel.SetActive(GameManager.Instance.isGamePaused);
    }


    private void FixedUpdate()
    {
        scoreTxt.text = $"Score: {GameManager.Instance.Score}";
    }

    private void ResumeClick()
    {
        GameManager.Instance.UnpauseGame();
    }

    private void QuitClick()
    {
        SceneManager.LoadScene("Title");
    }
}
