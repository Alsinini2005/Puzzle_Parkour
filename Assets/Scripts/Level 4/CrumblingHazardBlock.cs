using UnityEngine;
using System.Collections;

public class CrumblingHazardBlock : MonoBehaviour
{
    [Header("Settings")]
    public float breakDelay = 0f;
    public float destroyFragmentsAfter = 0f;
    public float respawnTime = 999999f;
    public GameObject fragmentPrefab; 

    [Header("Fall Trapping")]
    public float downwardPullForce = 15f;

    private bool isBreaking = false;
    private MeshRenderer meshRenderer;
    private Collider blockCollider;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        blockCollider = GetComponent<Collider>();

        if (fragmentPrefab == null)
        {
            fragmentPrefab = Resources.Load<GameObject>("Prefaps/CrumblingBlock");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isBreaking)
        {
            ApplyInstantFall(collision.gameObject);

            StartCoroutine(BreakSequence());
        }
    }

    IEnumerator BreakSequence()
    {
        isBreaking = true;

        if (breakDelay > 0f)
        {
            Vector3 originalPos = transform.position;
            float elapsed = 0f;
            while (elapsed < breakDelay)
            {
                transform.position = originalPos + (Random.insideUnitSphere * 0.08f);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        SpawnFourFragments();

        yield return new WaitForSeconds(respawnTime);
    }

    void ApplyInstantFall(GameObject player)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector3(0f, playerRb.linearVelocity.y, 0f);

            playerRb.AddForce(Vector3.down * downwardPullForce, ForceMode.VelocityChange);
        }
    }

    void SpawnFourFragments()
    {
        meshRenderer.enabled = false;
        blockCollider.enabled = false;

        if (fragmentPrefab != null && destroyFragmentsAfter > 0f)
        {
            GameObject frag = Instantiate(fragmentPrefab, transform.position, transform.rotation);
            Destroy(frag, destroyFragmentsAfter);
        }
    }
}