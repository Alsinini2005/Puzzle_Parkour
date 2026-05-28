using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer instance;

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 5)
        LoadSavedTime();
        
        StartTimer();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartTimer() => isTimerRunning = true;
    public void StopTimer() => isTimerRunning = false;
    public void ResetTimer() => elapsedTime = 0f;

    public float GetRawTime() => elapsedTime;

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SaveTime()
{
    PlayerPrefs.SetFloat("Level_5_SavedTime", elapsedTime);
    PlayerPrefs.Save();
}

public void LoadSavedTime()
{
    elapsedTime = PlayerPrefs.GetFloat("Level_5_SavedTime", 0f);
}

public void ClearSavedTime()
{
    PlayerPrefs.DeleteKey("Level_5_SavedTime");
    PlayerPrefs.Save();
}
}