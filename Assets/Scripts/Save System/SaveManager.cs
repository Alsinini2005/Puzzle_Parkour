using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public SaveData data;

    public Transform player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            data = SaveSystem.Load();
            if (data == null) data = new SaveData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame();
    }

    public void SaveGame()
    {
        if (player != null)
        {
            data.playerPosition =
                new SerializableVector3(
                    player.position.x,
                    player.position.y,
                    player.position.z
                );
        }

        data.currentScene =
            SceneManager.GetActiveScene().name;

        SaveSystem.Save(data);
    }

    public bool HasTrigger(string id)
    {
        return data.triggers.Contains(id);
    }

    public void SaveTrigger(string id)
    {
        if (!data.triggers.Contains(id))
        {
            data.triggers.Add(id);
            SaveGame();
        }
    }

    public bool HasItem(string id)
    {
        return data.collectedItems.Contains(id);
    }

    public void SaveItem(string id)
    {
        if (!data.collectedItems.Contains(id))
        {
            data.collectedItems.Add(id);
            SaveGame();
        }
    }

    public bool HasDoor(string id)
    {
        return data.openedDoors.Contains(id);
    }

    public void SaveDoor(string id)
    {
        if (!data.openedDoors.Contains(id))
        {
            data.openedDoors.Add(id);
            SaveGame();
        }
    }
}