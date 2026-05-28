using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Stats")]
    private int currentCoins = 0;
    public int totalCoinsInLevel = 3;
    private int deathCount = 0;
    private int totalDeathsAcrossGame = 0;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            totalDeathsAcrossGame = PlayerPrefs.GetInt("TotalDeaths", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AddDeath()
    {
        deathCount++;
        totalDeathsAcrossGame++;
        PlayerPrefs.SetInt("TotalDeaths", totalDeathsAcrossGame);
        PlayerPrefs.Save();
    }

    public int GetTotalDeaths()
    {
        return totalDeathsAcrossGame;
    }

    public void ResetTotalDeaths()
    {
        totalDeathsAcrossGame = 0;
        PlayerPrefs.SetInt("TotalDeaths", 0);
        PlayerPrefs.Save();
    }
public void ResetLevelStats()
{
    
    if (GameTimer.instance != null)
    {
        if (SceneManager.GetActiveScene().buildIndex != 5)
        {
            GameTimer.instance.ResetTimer();
        }
        GameTimer.instance.StartTimer();
    }
}

    public void ResetGhost()
{
    if (GhostManager.instance != null)
    {
        GhostManager.instance.ClearGhostData();
    }
}

    public void AddCoin()
    {
        currentCoins++;
    }

    public int GetCurrentCoins() => currentCoins;
    public int GetTotalCoins() => totalCoinsInLevel;
    public int GetDeathCount() => deathCount;

    public void SaveLevelData(int levelIndex, int score, int coins, float timeSpent)
    {
        PlayerPrefs.SetInt($"Level_{levelIndex}_Coins", coins);

        int oldHighScore = PlayerPrefs.GetInt($"Level_{levelIndex}_HighScore", 0);
        if (score > oldHighScore) PlayerPrefs.SetInt($"Level_{levelIndex}_HighScore", score);

        float oldBestTime = PlayerPrefs.GetFloat($"Level_{levelIndex}_BestTime", float.MaxValue);
        if (timeSpent < oldBestTime) PlayerPrefs.SetFloat($"Level_{levelIndex}_BestTime", timeSpent);

        PlayerPrefs.SetInt($"Level_{levelIndex}_Unlocked", 1);
        PlayerPrefs.Save();
    }

    public void SaveCheckpoint(int levelIndex, Vector3 position)
    {
        PlayerPrefs.SetFloat($"Lvl_{levelIndex}_CP_X", position.x);
        PlayerPrefs.SetFloat($"Lvl_{levelIndex}_CP_Y", position.y);
        PlayerPrefs.SetFloat($"Lvl_{levelIndex}_CP_Z", position.z);
        PlayerPrefs.SetInt($"Lvl_{levelIndex}_HasCP", 1);
        PlayerPrefs.Save();
    }

    public Vector3 LoadCheckpoint(int levelIndex, Vector3 defaultPosition)
    {
        if (PlayerPrefs.GetInt($"Lvl_{levelIndex}_HasCP", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat($"Lvl_{levelIndex}_CP_X");
            float y = PlayerPrefs.GetFloat($"Lvl_{levelIndex}_CP_Y");
            float z = PlayerPrefs.GetFloat($"Lvl_{levelIndex}_CP_Z");
            return new Vector3(x, y, z);
        }
        return defaultPosition;
    }

    public void ClearCheckpoint(int levelIndex)
    {
        PlayerPrefs.SetInt($"Lvl_{levelIndex}_HasCP", 0);
        PlayerPrefs.Save();
    }

    public void SaveTriggerEvent(string eventKey, bool isDone)
    {
        PlayerPrefs.SetInt(eventKey, isDone ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool LoadTriggerEvent(string eventKey)
    {
        return PlayerPrefs.GetInt(eventKey, 0) == 1;
    }

    public int GetLastGateReached()
    {
        return PlayerPrefs.GetInt("LastGateReached", 0);
    }

public int GetTotalScoreAcrossAllLevels()
    {
        int totalScore = 0;
        for (int i = 1; i <= 5; i++)
        {
            totalScore += PlayerPrefs.GetInt($"Level_{i}_HighScore", 0);
        }
        return totalScore;
    }

public float GetTotalTimeAcrossAllLevels()
{
    float totalTime = 0;
    for (int i = 1; i <= 5; i++)
    {
        float levelTime = PlayerPrefs.GetFloat($"Level_{i}_BestTime", 0f);
        totalTime += levelTime;
        Debug.Log($"Level {i} Best Time: {levelTime}");
    }
    return totalTime;
}

public void ResetLevelDataOnly(int levelIndex)
{
    PlayerPrefs.SetInt($"Lvl_{levelIndex}_HasCP", 0);
    
    PlayerPrefs.SetInt($"Level_{levelIndex}_Coins", 0);
    
    PlayerPrefs.Save();
}

public void ClearLevelPlayerPrefs(
    int levelIndex,
    List<string> saveIDs = null,
    List<string> coinSaveKeys = null)
{

    PlayerPrefs.DeleteKey($"Lvl_{levelIndex}_HasCP");
    PlayerPrefs.DeleteKey($"Lvl_{levelIndex}_CP_X");
    PlayerPrefs.DeleteKey($"Lvl_{levelIndex}_CP_Y");
    PlayerPrefs.DeleteKey($"Lvl_{levelIndex}_CP_Z");

    PlayerPrefs.DeleteKey($"Level_{levelIndex}_Coins");

    if (levelIndex == 5)
    {
        PlayerPrefs.DeleteKey("LastGateReached");
    }

    if (saveIDs != null)
    {
        foreach (string id in saveIDs)
        {
            if (string.IsNullOrEmpty(id)) continue;

            PlayerPrefs.DeleteKey(id + "_Locked");
            PlayerPrefs.DeleteKey(id + "_Open");

            PlayerPrefs.DeleteKey(id + "_Power");
            PlayerPrefs.DeleteKey(id);

            PlayerPrefs.DeleteKey(id + "_Active");

            PlayerPrefs.DeleteKey(id + "_Drained");

            PlayerPrefs.DeleteKey(id + "_Solved");
            PlayerPrefs.DeleteKey(id + "_Lights");

            for (int i = 0; i < 16; i++)
            {
                PlayerPrefs.DeleteKey(id + "_Light_" + i + "_Enabled");
                PlayerPrefs.DeleteKey(id + "_LightR_" + i);
                PlayerPrefs.DeleteKey(id + "_LightG_" + i);
                PlayerPrefs.DeleteKey(id + "_LightB_" + i);
            }
        }
    }

    if (coinSaveKeys != null)
    {
        foreach (string coinKey in coinSaveKeys)
        {
            if (string.IsNullOrEmpty(coinKey)) continue;

            PlayerPrefs.DeleteKey(coinKey + "_Saved");
            PlayerPrefs.DeleteKey(coinKey + "_X");
            PlayerPrefs.DeleteKey(coinKey + "_Y");
            PlayerPrefs.DeleteKey(coinKey + "_Z");
            PlayerPrefs.DeleteKey(coinKey + "_InSocket");
        }
    }

    PlayerPrefs.Save();

    Debug.Log($"[GameManager] Level data {levelIndex} has been successfully cleared." +
              $"(saveIDs: {saveIDs?.Count ?? 0} | coinKeys: {coinSaveKeys?.Count ?? 0})");
}
}
