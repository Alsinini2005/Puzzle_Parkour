using UnityEngine;
using System.Collections;

public class CoinSocket : MonoBehaviour, IInteractable
{
    public static int currentGlobalOrder = 1;

    [Header("Sequence Order")]
    public int socketOrderID = 1;

    [Header("Socket Settings")]
    public Transform coinPlacementPoint;
    public GameObject hintVisualObject;

    [Header("Animation Settings")]
    public Animator targetAnimator;
    public string animationTriggerName = "Activate";

    [Header("Laser Delay Settings")]
    public float delayBeforeLaser = 0.5f;

    [Header("Linked Laser")]
    public LaserGrower laserToActivate;

    [Header("Save Settings")]
    public string socketUniqueSaveKey;

    [Header("Target Coin")]
    public CoinItem targetCoinForThisSocket;

    [Header("Door Settings")]
    public Animator doorAnimator;
    public string doorOpenTrigger = "gate_open";

    private bool isActivated = false;
    [Header("Audio Settings")]
    public AudioSource doorAudioSource;
    public AudioClip doorOpenClip;

    private void Awake()
    {
        currentGlobalOrder = 1;
    }

    void Start()
    {
        if (GameManager.instance != null && !string.IsNullOrEmpty(socketUniqueSaveKey))
        {
            if (GameManager.instance.LoadTriggerEvent(socketUniqueSaveKey))
            {
                ApplySavedStateDirectly();
            }
        }
    }

    private void ApplySavedStateDirectly()
    {
        isActivated = true;

        if (currentGlobalOrder <= socketOrderID)
        {
            currentGlobalOrder = socketOrderID + 1;
        }

        if (hintVisualObject != null)
        {
            hintVisualObject.SetActive(false);
        }

        if (laserToActivate != null)
        {
            laserToActivate.StartGrowing();
        }

        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger(animationTriggerName);
        }

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(doorOpenTrigger);
        }

        if (targetCoinForThisSocket != null)
        {
            targetCoinForThisSocket.transform.SetParent(coinPlacementPoint);
            targetCoinForThisSocket.transform.localPosition = Vector3.zero;
            targetCoinForThisSocket.transform.localRotation = Quaternion.identity;

            Rigidbody rb = targetCoinForThisSocket.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider col = targetCoinForThisSocket.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            if (!string.IsNullOrEmpty(targetCoinForThisSocket.coinUniqueSaveKey))
            {
                PlayerPrefs.SetInt(targetCoinForThisSocket.coinUniqueSaveKey + "_InSocket", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Socket Interacted");

        if (isActivated)
        {
            Debug.Log("Already Activated");
            return;
        }

        Debug.Log("Socket ID: " + socketOrderID);
        Debug.Log("Current Global Order: " + currentGlobalOrder);

        if (socketOrderID != currentGlobalOrder)
        {
            LevelIntroController.Instance.ShowIntro(
                "[note]",
                $"You must activate socket number {currentGlobalOrder} first!"
            );

            return;
        }

        PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            Debug.Log("Player NOT Found");
            return;
        }

        if (player.holdItem == null)
        {
            Debug.Log("Player not holding item");
            return;
        }

        CoinItem coin = player.holdItem.GetComponent<CoinItem>();

        Debug.Log("Held Item: " + player.holdItem);
        Debug.Log("Coin Component: " + coin);
        Debug.Log("Target Coin: " + targetCoinForThisSocket);

        if (coin == null)
        {
            Debug.Log("Held item is NOT a coin");
            return;
        }

        player.holdItem = null;

        ObjectRespawn respawnScript = coin.GetComponent<ObjectRespawn>();
        if (respawnScript != null)
        {
            Destroy(respawnScript);
        }

        Transform coinTransform = coin.transform;

        coinTransform.SetParent(coinPlacementPoint);
        coinTransform.localPosition = Vector3.zero;
        coinTransform.localRotation = Quaternion.identity;

        Rigidbody coinRb = coin.GetComponent<Rigidbody>();

        if (coinRb != null)
        {
            coinRb.isKinematic = true;
            coinRb.useGravity = false;
        }

        Collider coinCol = coin.GetComponent<Collider>();

        if (coinCol != null)
        {
            coinCol.enabled = false;
        }

        if (hintVisualObject != null)
        {
            hintVisualObject.SetActive(false);
        }

        isActivated = true;

        currentGlobalOrder++;

        Debug.Log("Socket Activated Successfully");

        if (!string.IsNullOrEmpty(coin.coinUniqueSaveKey))
        {
            PlayerPrefs.SetInt(coin.coinUniqueSaveKey + "_InSocket", 1);
            PlayerPrefs.Save();
        }

        if (GameManager.instance != null && !string.IsNullOrEmpty(socketUniqueSaveKey))
        {
            GameManager.instance.SaveTriggerEvent(socketUniqueSaveKey, true);
        }

        StartCoroutine(AnimationThenFireSequence());
    }

    IEnumerator AnimationThenFireSequence()
    {
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger(animationTriggerName);
        }

        yield return new WaitForSeconds(delayBeforeLaser);

        if (laserToActivate != null)
        {
            laserToActivate.StartGrowing();
        }

        yield return new WaitForSeconds(0.1f);

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(doorOpenTrigger);
        }

        if (doorAudioSource != null && doorOpenClip != null)
    {
        doorAudioSource.clip = doorOpenClip;
        doorAudioSource.Play();
    }
    }
}