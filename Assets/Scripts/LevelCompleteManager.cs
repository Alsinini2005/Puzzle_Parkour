using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject statsParentObject;
    public Button nextLevelButton;
    public Button mainMenuButton;
    public TextMeshProUGUI winText;
    public VerticalLayoutGroup buttonsLayoutGroup;

    [Header("Stats Text Components")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI scoreText;
    [Header("Next Level Settings")]
    public int nextSceneBuildIndex;

    [Header("Score System Settings")]
    public int baseTimeBonus = 5000;
    public int scorePerCoin = 1000;
    public int penaltyPerDeath = 500;
    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
    }
    public void PlayerWin(int collectedCoins, int totalCoins, int deathCount, bool isLastLevel)
{ 
    float finalTime = 0f;
    if (GameTimer.instance != null)
    {
        GameTimer.instance.StopTimer();
        finalTime = GameTimer.instance.GetRawTime();
    }

    int timeBonus = Mathf.Max(0, baseTimeBonus - Mathf.FloorToInt(finalTime * 10));
    int finalScore = (collectedCoins * scorePerCoin) + timeBonus - (deathCount * penaltyPerDeath);
    
    if (GameManager.instance != null)
    {
        GameManager.instance.SaveLevelData(SceneManager.GetActiveScene().buildIndex, finalScore, collectedCoins, finalTime);
    }

    if (winPanel != null) winPanel.SetActive(true);

    if (isLastLevel)
    {
        int totalAllLevels = GameManager.instance.GetTotalScoreAcrossAllLevels();
        float totalAllTime = GameManager.instance.GetTotalTimeAcrossAllLevels();
        int totalDeaths = GameManager.instance.GetTotalDeaths();

        string totalTimeStr = FormatTime(totalAllTime);

        winText.text = $"LEVEL 5: CONGRATULATIONS!\n\n" +
                       $"YOU FINISHED THE GAME!\n\n" +
                       $"TOTAL SCORE: {totalAllLevels}\n" +
                       $"TOTAL TIME: {totalTimeStr}\n" +
                       $"TOTAL DEATHS: {totalDeaths}";

        if (statsParentObject != null) statsParentObject.SetActive(false); 

        if (buttonsLayoutGroup != null)
        {
            RectOffset newPadding = buttonsLayoutGroup.padding;
            newPadding.bottom = -200;
            newPadding.top = 145;
            buttonsLayoutGroup.padding = newPadding;
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonsLayoutGroup.GetComponent<RectTransform>());
        }
    }
    else
    {
        float bestTime = PlayerPrefs.GetFloat($"Level_{SceneManager.GetActiveScene().buildIndex}_BestTime", finalTime);
        
        string timeStr = FormatTime(finalTime);
        string bestTimeStr = FormatTime(bestTime);

        winText.text = $"LEVEL COMPLETE!\n\n" +
                       $"CURRENT TIME: {timeStr}\n" +
                       $"BEST TIME: {bestTimeStr}\n" +
                       $"COINS: {collectedCoins} / {totalCoins}\n" +
                       $"SCORE: {finalScore}\n" +
                       $"DEATHS: {deathCount}";
        
        if (statsParentObject != null) statsParentObject.SetActive(true);
        if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(true);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(false);
    }
    
    Time.timeScale = 0f; 
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

void CalculateAndDisplayScore(int coins, int totalCoins, int deaths, float timeSpent)
{
    if (SceneManager.GetActiveScene().buildIndex == 5) return;

    if (coinsText != null)
    {
        coinsText.text = $"COINS: {coins} / {totalCoins}";
    }

    int timeBonus = Mathf.Max(0, baseTimeBonus - Mathf.FloorToInt(timeSpent * 10));
    int finalScore = (coins * scorePerCoin) + timeBonus - (deaths * penaltyPerDeath);
    finalScore = Mathf.Max(0, finalScore);
 
    if (scoreText != null)
    {
        scoreText.text = $"FINAL SCORE: {finalScore}";
    }
}

    public void NextLevel()
    {
        Time.timeScale = 1f;
        if (GameManager.instance != null) GameManager.instance.ResetLevelStats();
        
        if (nextSceneBuildIndex >= 0 && nextSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneBuildIndex);
        }
        else
        {
            Debug.LogError("Scene Number " + nextSceneBuildIndex + " not found in Build Settings!");
        }
    }

public void RestartLevel()
{
    Time.timeScale = 1f;
    int currentLevel = SceneManager.GetActiveScene().buildIndex;

    if (GameManager.instance != null)
    {
        List<string> mySaveIDs = new List<string> { 
            "Lvl2_Coin_01", "Lvl2_Socket1", "Lvl2_Socket2", "Lvl2_Coin_02", "box_level_2_save_1", "DOOR_1", "power_room", "box_level5_1_gate_two",
             "puzzle_gate_02", "level_5_gate_1_lever", "level_5_Lever3_Drain", "level5_gate_3_box_1"
        };
        
        List<string> myCoinKeys = null;

        GameManager.instance.ClearLevelPlayerPrefs(currentLevel, mySaveIDs, myCoinKeys);
        
        GameManager.instance.ResetLevelStats();
    }

    SceneManager.LoadScene(currentLevel);
}

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private string FormatTime(float timeInSeconds)
    {
        int m = Mathf.FloorToInt(timeInSeconds / 60F);
        int s = Mathf.FloorToInt(timeInSeconds % 60F);
        return string.Format("{0:00}:{1:00}", m, s);
    }
}