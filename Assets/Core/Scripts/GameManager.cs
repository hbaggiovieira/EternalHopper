using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    [HideInInspector]
    public int Floor { get; set; }
    
    [HideInInspector]
    public int ComboCounter { get; set; } = 0;

    [HideInInspector]
    public float CameraSpeed { get; private set; } = 5f;
    private const float CameraSpeedRaiseConstant = 2f;

    public static GameManager Instance { get; private set; }

    public bool isGamePaused { get; private set; } = false;

    [HideInInspector]
    public int LevelUpPlatformConstant { get; private set; } = 100;

    [HideInInspector]
    public int RunningSpeedLevel { get; set; } = 1;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Floor = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isGamePaused = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        isGamePaused = false;
    }

    public void RestartGame()
    {
        UnpauseGame();
        Floor = 0;
        SceneManager.LoadScene("Play");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void RaiseCameraSpeed()
    {
        CameraSpeed += CameraSpeedRaiseConstant;
    }
}
