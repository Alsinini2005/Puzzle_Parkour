using UnityEngine;

public class BallBasket : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 2f;
    public int maxBallsInBasket = 1;

    private int currentBalls;

    void Start()
    {
        InvokeRepeating("AttemptSpawn", 1f, spawnDelay);
    }

    void AttemptSpawn()
{
    Collider[] colliders = Physics.OverlapSphere(spawnPoint.position, 1.0f);
    int ballCount = 0;
    
    PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();

    foreach (var col in colliders)
    {
        if (col.CompareTag("Ball")) 
        {
            if (player != null && player.holdItem == col.gameObject) continue;
            
            ballCount++;
        }
    }

    if (ballCount < maxBallsInBasket)
    {
        Instantiate(ballPrefab, spawnPoint.position, Random.rotation);
    }
}
}