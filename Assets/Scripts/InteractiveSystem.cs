using UnityEngine;

public class InteractiveSystem : MonoBehaviour, IInteractable
{
    [Header("Setup")]
    public Transform doorMesh;
    public bool isLocked = true;
    public bool isOpen = false;
    public bool canBeOpenedByUser = true;

    [Header("Animation Names")]
    public string openClipName = "DoorOpen";
    public string slamClipName = "DoorSlam";

    [Header("Save System")]
    public string saveID;

    private AudioSource audioSource;
    private Animator anim;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        if (doorMesh != null)
        {
            anim = doorMesh.GetComponent<Animator>();
        }
    }

    void Start()
    {
        LoadState();
    }

    void PlaySound(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + fileName);

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void Interact()
    {
        if (isOpen) return;

        PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();

        if (isLocked)
        {
            if (player.holdItem != null &&
                player.holdItem.GetComponent<KeyItem>() != null)
            {
                if (player.holdItem.GetComponent<KeyItem>().doorSystem == this)
                {
                    UnlockAndOpen(player);
                }
            }
            else
            {
                PlaySound("locking-door-454241");
            }

            return;
        }

        ToggleDoor();
    }

    public void ForceToggle()
    {
        if (isOpen) return;

        isLocked = false;

        ToggleDoor();

        Debug.Log("The door was opened by an external trigger/button!");

        PlaySound("open-doors-114615");

        SaveState();
    }

    void UnlockAndOpen(PlayerController player)
    {
        isLocked = false;

        PlaySound("open-doors-114615");

        KeyItem key = player.holdItem.GetComponent<KeyItem>();

        key.DropBox();

        player.holdItem = null;

        ToggleDoor();

        SaveState();
    }

    void ToggleDoor()
    {
        isOpen = true;

        PlaySound("open-doors-114615");

        if (anim != null)
        {
            anim.Play(openClipName);
        }

        SaveState();
    }

    public void CloseDoorSlam()
    {
        if (!isOpen) return;

        isOpen = false;
        isLocked = true;

        if (anim != null)
        {
            anim.Play(slamClipName);
        }

        PlaySound("door-slam-172171");

        SaveState();
    }

    void SaveState()
    {
        PlayerPrefs.SetInt(saveID + "_Locked", isLocked ? 1 : 0);
        PlayerPrefs.SetInt(saveID + "_Open", isOpen ? 1 : 0);

        PlayerPrefs.Save();
    }

    void LoadState()
    {
        if (string.IsNullOrEmpty(saveID)) return;

        if (PlayerPrefs.HasKey(saveID + "_Locked"))
        {
            isLocked = PlayerPrefs.GetInt(saveID + "_Locked") == 1;
        }

        if (PlayerPrefs.HasKey(saveID + "_Open"))
        {
            isOpen = PlayerPrefs.GetInt(saveID + "_Open") == 1;

            if (isOpen && anim != null)
            {
                anim.Play(openClipName, 0, 1f);
            }
        }
    }
}