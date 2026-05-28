using UnityEngine;

public class CoinPlatformTrigger : MonoBehaviour
{
    [Header("Target Platform")]
    public MonoBehaviour movingPlatformScript;

    [Header("Customization Settings")]
    public bool destroyCoinOnHit = true;

    public GameObject hitEffectPrefab;

    private bool hasBeenHit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBeenHit && (collision.gameObject.CompareTag("Ball") || collision.gameObject.GetComponent<BallItem>() != null))
        {
            hasBeenHit = true;

            ActivatePlatform();

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, transform.rotation);
            }

            if (destroyCoinOnHit)
            {
                Destroy(gameObject);
            }
            else
            {
                GetComponent<Collider>().enabled = false;
                if (GetComponent<MeshRenderer>()) GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void ActivatePlatform()
    {
        if (movingPlatformScript != null)
        {
            movingPlatformScript.enabled = true;

            movingPlatformScript.Invoke("StartMoving", 0f); 
        }
    }
}