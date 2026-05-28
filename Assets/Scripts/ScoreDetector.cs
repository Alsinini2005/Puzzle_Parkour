using UnityEngine;

public class ScoreDetector : MonoBehaviour
{
    [Header("Level Settings")]
    public int levelNumber = 1;
    public GameObject cylinderToShow;

    [Header("Level 3 Settings")]
    public MonoBehaviour movingPlatformScript; 

    private void OnTriggerEnter(Collider other)
    {
        bool isBall = other.CompareTag("Ball") || other.GetComponent<BallItem>() != null;
        bool isPlayer = other.CompareTag("Player");

        if (isPlayer){
            GameManager.instance.AddCoin();
            Destroy(gameObject);
        }

        if (levelNumber == 1 && isBall)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddCoin();
            }

            Destroy(gameObject); 

            if (cylinderToShow != null)
            {
                cylinderToShow.SetActive(true);
            }
        }

        if (levelNumber == 3 && isBall)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddCoin();
            }

            Destroy(gameObject); 

            if (movingPlatformScript != null)
            {
                movingPlatformScript.enabled = true;
                movingPlatformScript.Invoke("StartMoving", 0f);
            }
        }
    }
}