using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostManager : MonoBehaviour
{
    public static GhostManager instance;
    [SerializeField] private List<GhostFrame> savedGhostData = new List<GhostFrame>();
    public int recordedLevelIndex = -1;

    public bool HasGhostData => savedGhostData != null && savedGhostData.Count > 0;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

public void SetGhostData(List<GhostFrame> data, int levelIndex)
    {
        if (data != null && data.Count > 0)
        {
            savedGhostData = new List<GhostFrame>(data);
            recordedLevelIndex = levelIndex;
        }
    }

    public List<GhostFrame> GetGhostData()
    {
        return new List<GhostFrame>(savedGhostData);
    }

    public void ClearGhostData()
    {
        savedGhostData.Clear();
    }

    public void PrintStatus()
    {
        Debug.Log($"📊 GhostManager Status: {savedGhostData.Count} frame saved");
    }
}