public static class GameEvents
{
    public delegate void OnEndGameEvent();
    public static event OnEndGameEvent OnEndGame;

    public static void TriggerEndGameEvent()
    {
        Highscores.SaveHighscore(GameManager.Instance.Score);

        OnEndGame?.Invoke();
    }
}