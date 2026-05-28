using UnityEngine;
using System.Collections;

public class CrumblingBlock : MonoBehaviour
{
    [Header("Settings")]
    public float breakDelay = 0.5f;
    public float destroyFragmentsAfter = 4f;
    public float respawnTime = 3.5f;
    public GameObject fragmentPrefab; 

    private bool isBreaking = false;
    private MeshRenderer meshRenderer;
    private Collider blockCollider;
    private Vector3 originalPosition;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        blockCollider = GetComponent<Collider>();
        originalPosition = transform.position;

        if (fragmentPrefab == null)
        {
            fragmentPrefab = Resources.Load<GameObject>("Prefaps/CrumblingBlock");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isBreaking)
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                StartCoroutine(BreakSequence());
            }
        }
    }

    IEnumerator BreakSequence()
    {
        isBreaking = true;

        Vector3 originalPos = transform.position;
        float elapsed = 0f;
        while (elapsed < breakDelay)
        {
            transform.position = originalPos + (Random.insideUnitSphere * 0.05f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        SpawnFourFragments();

        yield return new WaitForSeconds(respawnTime);
        ResetBlock();
    }

    void SpawnFourFragments()
    {
        if (fragmentPrefab == null) return;

        meshRenderer.enabled = false;
        blockCollider.enabled = false;

        float xOffset = 0.75f;
        float zOffset = 0.75f;

        Vector3[] offsets = new Vector3[]
        {
            new Vector3(xOffset, 0f, zOffset),
            new Vector3(-xOffset, 0f, zOffset),
            new Vector3(xOffset, 0f, -zOffset),
            new Vector3(-xOffset, 0f, -zOffset)
        };

        foreach (Vector3 offset in offsets)
        {
            Quaternion randomRotation = transform.rotation * Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            GameObject frag = Instantiate(fragmentPrefab, transform.position + offset, randomRotation);
            
            Rigidbody rb = frag.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddTorque(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), ForceMode.Impulse);
                rb.AddForce((offset.normalized + Vector3.down) * Random.Range(1f, 3f), ForceMode.Impulse);
            }

            Destroy(frag, destroyFragmentsAfter);
        }
    }

    void ResetBlock()
    {
        transform.position = originalPosition;
        meshRenderer.enabled = true;
        blockCollider.enabled = true;
        isBreaking = false;
    }
}