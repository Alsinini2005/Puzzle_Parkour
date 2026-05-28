using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTracker : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("LastLevel", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();
    }
}