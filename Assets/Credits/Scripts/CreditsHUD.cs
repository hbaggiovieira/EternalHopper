using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsHUD : MonoBehaviour
{
    public Button closeBtn;

    private void Start()
    {
        closeBtn.onClick.RemoveAllListeners();

        closeBtn.onClick.AddListener(CloseClick);
    }

    private void CloseClick()
    {
        SceneManager.LoadScene("Title");
    }
}
