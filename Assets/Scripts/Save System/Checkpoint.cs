using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int levelNumber = 1;
    private bool isActivated = false;

    [Header("Checkpoint ID")]
    public string checkpointID = ""; 

    [Header("Lava Settings")]
    public LavaRiser lava1;
    public LavaRiser lava2;

private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player") && !isActivated)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveCheckpoint(levelNumber, other.transform.position);
            isActivated = true;
            if (checkpointID == "LavaTrigger")
                ActivateLava();
        }
    }
}

    private void ActivateLava()
    {
        if (LevelIntroController.Instance != null)
        {
            LevelIntroController.Instance.ShowIntro("[WARNING]", "*RUN FAST!!*");
        }
        
        if (lava1 != null) lava1.isRising = true;
        if (lava2 != null) lava2.isRising = true;
    }
}