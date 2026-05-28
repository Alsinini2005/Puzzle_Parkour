using UnityEngine;

public class PickableBox : MonoBehaviour, IInteractable, IPickupable
{
    private Rigidbody rb;
    private Collider col;

    private bool isHeld = false;
    private Transform holdPoint;

    [Header("Save Settings")]
    public string boxUniqueSaveKey;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void FixedUpdate()
{
    if (!isHeld || holdPoint == null) return;

    PlayerController player = FindFirstObjectByType<PlayerController>();
    if (player == null || player.holdItem != gameObject) return;

    Vector3 targetPos = player.holdPoint.position;
    Vector3 direction = targetPos - transform.position;
    
    rb.linearVelocity = direction * 15f;
}

    public void Interact()
    {
        PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();

        if (player != null && player.holdItem == null)
        {
            PickUp(player);
        }
    }

    private void PickUp(PlayerController player)
{
    isHeld = true;
    holdPoint = player.holdPoint;

    rb.isKinematic = false;
    rb.useGravity = false;

    player.holdItem = gameObject;
}

    public void DropBox()
{
    isHeld = false;
    transform.SetParent(null);

    PlayerController player = FindFirstObjectByType<PlayerController>();
    if (player != null)
    {
        player.holdItem = null;
    }

    if (rb != null)
    {
        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Camera playerCam = Camera.main;

        Vector3 forward = playerCam != null ? playerCam.transform.forward : transform.forward;
        forward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;

        Vector3 throwForce = forward + Vector3.up;

        rb.AddForce(throwForce, ForceMode.Impulse);
    }

    if (col != null)
    {
        col.enabled = true;
        col.isTrigger = false;
    }
}

    public void OnPickedUp(Transform parent) 
    {
         PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();

        if (player != null && player.holdItem == null)
        {
            PickUp(player);
        }
    }
}