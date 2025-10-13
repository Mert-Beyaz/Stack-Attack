using UnityEngine;

public static class SaveSystem
{
    private const string LevelKey = "CurrentLevel";

    public static void SaveLevel(int level)
    {
        PlayerPrefs.SetInt(LevelKey, level);
        PlayerPrefs.Save();
    }

    public static int LoadLevel()
    {
        return PlayerPrefs.GetInt(LevelKey, 0);
    }

    public static void NextLevel()
    {
        int newLevel = LoadLevel() + 1;
        SaveLevel(newLevel);
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LevelKey);
    }
}
