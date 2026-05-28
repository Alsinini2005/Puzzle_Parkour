using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnHandler : MonoBehaviour
{
    public int currentLevelNumber = 1;

    void Start()
    {
        if (GameManager.instance != null)
        {
            Vector3 savedPos = GameManager.instance.LoadCheckpoint(currentLevelNumber, transform.position);
            
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            
            transform.position = savedPos;
            
            if (cc != null) cc.enabled = true;
        }
    }
}