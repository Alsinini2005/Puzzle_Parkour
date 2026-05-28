using UnityEngine;

public class KeyItem : MonoBehaviour, IInteractable, IPickupable
{
    public InteractiveSystem doorSystem;
    private Rigidbody rb;
    private Collider col;

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void Interact()
    {
        OnPickedUp(null);
    }

void FixedUpdate()
{
    PlayerController player = FindFirstObjectByType<PlayerController>();
    if (player == null || player.holdItem != gameObject) return;
    
    Vector3 targetPos = player.holdPoint.position;
    Vector3 direction = targetPos - transform.position;
    float distance = direction.magnitude;

    float speed = Mathf.Clamp(distance * 15f, 0f, 20f);
    rb.linearVelocity = direction.normalized * speed;
}

public void OnPickedUp(Transform parent) 
{
    PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();
    if (player != null && player.holdItem == null)
    {
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
        }
        
        transform.SetParent(player.holdPoint);
        transform.localPosition = Vector3.zero;
        player.holdItem = gameObject;
        player.heldItemRotation = new Vector3(0f, 180f, 0f);
        LevelIntroController.Instance.ShowIntro("[note]", "Key picked up. Go to the door.");
    }
}

    public void DropBox()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        Camera playerCam = Camera.main;

        if (player != null)
        {
            player.heldItemRotation = new Vector3(0f, -100f, 0f);
            player.holdItem = null;
        }

        transform.SetParent(null);
        transform.position = playerCam != null
            ? playerCam.transform.position + playerCam.transform.forward * 1.5f 
            : transform.position;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x, 
                90f, 
                transform.rotation.eulerAngles.z
            );

            Vector3 throwDirection = playerCam != null ? playerCam.transform.forward : transform.forward;
            rb.AddForce(throwDirection * 5f + Vector3.up * 2f, ForceMode.Impulse);
        }

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
        }
    }
private System.Collections.IEnumerator EnableColliderDelayed()
{
    yield return new WaitForSeconds(0.15f);
    if (col != null)
    {
        col.enabled = true;
        col.isTrigger = false;
    }
}
}