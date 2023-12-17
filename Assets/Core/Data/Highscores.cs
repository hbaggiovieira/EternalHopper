
using UnityEngine;

public class Highscores : MonoBehaviour
{
    private const int MaxHighscores = 5;
    private const string HighscoreKey = "Highscores";

    public static void SaveHighscore(int newScore)
    {

        HighscoreList highscoreList = LoadHighscores();

        bool shouldAddScore = highscoreList.highscores.Count < MaxHighscores;

        for (int i = 0; i < highscoreList.highscores.Count; i++)
        {
            if (newScore > highscoreList.highscores[i])
            {
                highscoreList.highscores.Insert(i, newScore);

                shouldAddScore = false;
                break;
            }
        }

        if (shouldAddScore)
        {
            highscoreList.highscores.Add(newScore);
        }

        if (highscoreList.highscores.Count > MaxHighscores)
        {
            highscoreList.highscores.RemoveRange(MaxHighscores, highscoreList.highscores.Count - MaxHighscores);
        }

        string json = JsonUtility.ToJson(highscoreList);
        PlayerPrefs.SetString(HighscoreKey, json);
        PlayerPrefs.Save();
    }

    public static HighscoreList LoadHighscores()
    {
        string json = PlayerPrefs.GetString(HighscoreKey, "{}");
        return JsonUtility.FromJson<HighscoreList>(json);
    }
}