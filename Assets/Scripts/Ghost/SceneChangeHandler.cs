using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeHandler : MonoBehaviour
{
    public static SceneChangeHandler instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("✅ SCENE LOADED: " + scene.name + " | index: " + scene.buildIndex);

        StartCoroutine(TriggerGhostSpawning());
    }

    System.Collections.IEnumerator TriggerGhostSpawning()
    {
        yield return null;
        yield return null;

        if (GhostManager.instance == null) yield break;

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int recordedLevel = GhostManager.instance.recordedLevelIndex;

        if (currentLevel != recordedLevel)
        {
            Debug.Log($"⏭ Skipping ghost: current={currentLevel}, recorded={recordedLevel}");
            yield break;
        }

        GhostSpawner spawner = FindFirstObjectByType<GhostSpawner>();
        if (spawner != null)
        {
            spawner.PrepareForGhost();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}