using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class FinishLineTrigger : MonoBehaviour
{
    private bool isFinished = false;

private void OnTriggerEnter(Collider other)
{
    if (isFinished || !other.CompareTag("Player")) return;
    isFinished = true;

    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    int nextScene = currentSceneIndex + 1;
    PlayerPrefs.SetInt("LastLevel", nextScene);
    PlayerPrefs.Save();

    GhostRecorder recorder = other.GetComponent<GhostRecorder>();
    if (recorder != null)
    {
        recorder.SaveToGhostManager();
    }

    var winManager = FindFirstObjectByType<LevelCompleteManager>();
    if (winManager != null && GameManager.instance != null)
    {
        bool isLastLevel = (currentSceneIndex == 5); 
        winManager.PlayerWin(
            GameManager.instance.GetCurrentCoins(),
            GameManager.instance.GetTotalCoins(),
            GameManager.instance.GetDeathCount(),
            isLastLevel
        );
    }
}

    System.Collections.IEnumerator LoadNextScene()
{
    yield return new WaitForSecondsRealtime(2f);

    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    int nextSceneIndex = currentSceneIndex + 1;

    Debug.Log($"Current Scene: {currentSceneIndex}");
    Debug.Log($"Next Scene: {nextSceneIndex}");

    SceneManager.LoadScene(nextSceneIndex);
}
}