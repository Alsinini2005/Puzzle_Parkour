using UnityEngine;

public class BallItem : MonoBehaviour, IInteractable, IPickupable
{
    private Rigidbody rb;
    private Collider col;

    [Header("Throw Settings")]
    public float throwForce = 15f;
    public float upForce = 3f;
    public float rotationForce = 20f;
    private Vector3 savedScale;

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        savedScale = transform.localScale; 
    }

    public void Interact() 
    {
        PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();
        
        if (player != null && player.holdItem == null)
        {
            if (rb) rb.isKinematic = true;
            if (col) col.enabled = false;

            transform.SetParent(player.holdPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            
            player.holdItem = gameObject;
        }
    }

public void DropBox()
{
    transform.SetParent(null);

    if (rb != null)
    {
        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Camera playerCam = Camera.main;
        Vector3 throwDirection = playerCam != null ? playerCam.transform.forward : transform.forward;

        rb.AddForce(throwDirection * 15f + Vector3.up * 3f, ForceMode.Impulse);
    }

    if (col != null)
    {
        col.enabled = true;
        col.isTrigger = false;
    }

    PlayerController player = FindFirstObjectByType<PlayerController>();
    if (player != null)
        player.holdItem = null;

    
    Destroy(gameObject, 5.0f);
}

public void Drop()
{
    transform.SetParent(null); 
    if (rb != null) 
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
    }
    if (col != null) col.enabled = true;
}

public void OnPickedUp(Transform parent) 
{
    PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();
    
    if (player != null && player.holdItem == null)
    {
        if (rb) rb.isKinematic = true;
        if (col) col.enabled = false;

        Vector3 worldScale = transform.lossyScale;

        transform.SetParent(player.holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );
        
        player.holdItem = gameObject;
    }
}
}