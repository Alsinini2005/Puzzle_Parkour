using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public static bool isGamePaused = false;
    
    [Header("Failed Menu Settings")]
    public GameObject failedPanel;
    public Animator failedAnimator;

    [Header("Camera Shake Settings")]
    public float defaultShakeDuration = 0.5f;  
    public float defaultShakeMagnitude = 0.2f; 

    [Header("In-Game Gameplay UI")]
    public TextMeshProUGUI gameplayCoinsText;
    public TextMeshProUGUI gameplayScoreText;
    public TextMeshProUGUI gameplayTimerText;
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip gameOverClip;
    private bool isFailing = false;

    void Start()
    {
        isFailing = false;
        Time.timeScale = 1f;
        isGamePaused = false;
        if (failedPanel != null) failedPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (failedPanel) failedPanel.SetActive(false);
        
    }

    void Update()
    {
        if (failedPanel == null || failedPanel.activeSelf) return;

        UpdateGameplayUI();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isGamePaused) Resume();
            else Pause();
        }
    }

    void UpdateGameplayUI()
    {
        if (GameManager.instance != null)
        {
            if (gameplayCoinsText != null)
                gameplayCoinsText.text = "Coins: " + GameManager.instance.GetCurrentCoins() + " / " + GameManager.instance.GetTotalCoins();

            if (gameplayScoreText != null)
            {
                int currentCoins = GameManager.instance.GetCurrentCoins();
                int tempScore = currentCoins * 1000;

                if (GameTimer.instance != null)
                {
                    float timeSpent = GameTimer.instance.GetRawTime();
                    tempScore += Mathf.Max(0, 5000 - Mathf.FloorToInt(timeSpent * 10));
                }

                tempScore = Mathf.Max(0, tempScore - (GameManager.instance.GetDeathCount() * 500));
                gameplayScoreText.text = "Score: " + tempScore;
            }

            if (gameplayTimerText != null && GameTimer.instance != null)
        {
            gameplayTimerText.text = "Time: " + GameTimer.instance.GetFormattedTime();
        }
        }
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
        StartCoroutine(SyncCursorState(false));
    }

    void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        StartCoroutine(SyncCursorState(true));
    }

    System.Collections.IEnumerator SyncCursorState(bool visible)
    {
        yield return new WaitForEndOfFrame();
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    public void TriggerFail()
{
    if (isFailing) return;
    isFailing = true;

    if (audioSource != null && gameOverClip != null)
    {
            audioSource.PlayOneShot(gameOverClip);
    }

    if (failedPanel != null) 
    {
        failedPanel.SetActive(true); 
    }

    if (failedAnimator != null) 
    {
        failedAnimator.ResetTrigger("Fail"); 
        failedAnimator.SetTrigger("Fail");
    }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(ShakeCamera(defaultShakeDuration, defaultShakeMagnitude));
        Invoke(nameof(FreezeTime), 0.1f);
    }

    void FreezeTime() => Time.timeScale = 0f;

    public System.Collections.IEnumerator ShakeCamera(float duration, float magnitude)
    {
        if (Camera.main == null) yield break;
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }

public void RestartLevel()
{
    CancelInvoke();
    Time.timeScale = 1f;
    isFailing = false;

    if (failedPanel != null) failedPanel.SetActive(false);
    
    if (GameManager.instance != null)
    {
        GameManager.instance.ResetLevelStats();
    }

    int currentLevel = SceneManager.GetActiveScene().buildIndex;
    SceneManager.LoadScene(currentLevel);
}

    public void GoToMainMenu()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 5)
            if (GameTimer.instance != null)
                GameTimer.instance.SaveTime();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
    if (settingsPanel != null)
    {
        settingsPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    }

public void CloseSettings()
{
    if (settingsPanel != null)
    {
        settingsPanel.SetActive(false);
        if (isGamePaused && pausePanel != null) pausePanel.SetActive(true);
    }
}
}