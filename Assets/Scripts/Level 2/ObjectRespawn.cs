using UnityEngine;

public class ObjectRespawn : MonoBehaviour
{
    public string killZoneTag = "KillZone";

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(killZoneTag))
        {
            if (transform.parent != null)
            {
                PlayerController player = transform.parent.GetComponentInParent<PlayerController>();
                if (player != null && player.holdItem == gameObject)
                {
                    player.holdItem = null;
                }
                transform.SetParent(null);
            }

            transform.position = originalPosition;
            transform.rotation = originalRotation;

            if (rb)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Collider col = GetComponent<Collider>();
            if (col) col.enabled = true;

        }
    }
}