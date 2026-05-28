using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Tooltip("Scene Number")]
    public int targetSceneNum;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);

            SceneManager.LoadScene(targetSceneNum);
        }
    }
}