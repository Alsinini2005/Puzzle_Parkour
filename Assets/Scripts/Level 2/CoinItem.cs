using UnityEngine;

public class CoinItem : MonoBehaviour, IInteractable, IPickupable
{
    private Rigidbody rb;
    private Collider col;

    [Header("Save Settings")]
    public string coinUniqueSaveKey;

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        LoadCoinPosition();
    }

    private void LoadCoinPosition()
    {
        if (GameManager.instance != null && !string.IsNullOrEmpty(coinUniqueSaveKey))
        {
            if (PlayerPrefs.GetInt(coinUniqueSaveKey + "_InSocket", 0) == 1)
            {
                return;
            }

            if (PlayerPrefs.GetInt(coinUniqueSaveKey + "_Saved", 0) == 1)
            {
                float x = PlayerPrefs.GetFloat(coinUniqueSaveKey + "_X");
                float y = PlayerPrefs.GetFloat(coinUniqueSaveKey + "_Y");
                float z = PlayerPrefs.GetFloat(coinUniqueSaveKey + "_Z");
                
                transform.position = new Vector3(x, y, z);
            }
        }
    }

    public void SaveCoinPosition()
    {
        if (GameManager.instance != null && !string.IsNullOrEmpty(coinUniqueSaveKey))
        {
            PlayerPrefs.SetFloat(coinUniqueSaveKey + "_X", transform.position.x);
            PlayerPrefs.SetFloat(coinUniqueSaveKey + "_Y", transform.position.y);
            PlayerPrefs.SetFloat(coinUniqueSaveKey + "_Z", transform.position.z);
            PlayerPrefs.SetInt(coinUniqueSaveKey + "_Saved", 1);
            PlayerPrefs.Save();
        }
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
}
    public void OnPickedUp(Transform parent) 
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
}