using UnityEngine;
using System.Collections;

public class InteractionTrigger : MonoBehaviour, IInteractable
{
    [Header("Target Settings (Box/Door)")]
    public InteractiveSystem targetDoor;
    public Animator targetAnimator;
    public string animationToPlay = "BoxMove";

    [Header("Power Settings (Lights)")]
    public GameObject allLightsParent;
    public bool activatePowerOnInteract = true;

    [Header("Audio Settings")]
    public string powerOnSoundName = "power-on-sequence";
    private AudioSource audioSource;

    [Header("Activation Extras")]
    public Collider colliderToEnable;

    [Header("Visuals")]
    public Transform leverHandle;
    public LaserGrower linkedLaser;

    [Header("Specific Light Color")]
    public Light targetLight;
    public Color powerOnColor = Color.blue;

    [Header("Save System")]
    public string saveID;

    private bool isActivated = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    IEnumerator Start()
    {
        if (allLightsParent != null)
        {
            allLightsParent.SetActive(false);
        }

        yield return null;

        LoadState();
    }

    public void Interact()
    {
        if (isActivated) return;

        if (targetDoor != null)
        {
            targetDoor.ForceToggle();

            if (targetDoor.isOpen)
            {
                ActivateTrigger();
            }
        }
        else if (targetAnimator != null)
        {
            targetAnimator.Play(animationToPlay);

            ActivateTrigger();
        }
        else
        {
            ActivateTrigger();
        }
    }

    void ActivateTrigger()
    {
        isActivated = true;
        SaveState();
        RotateHandle();

        EnableTargetCollider();

        ChangeLightColor();

        if (activatePowerOnInteract)
        {
            TogglePower(true);

            PlayPowerSound(powerOnSoundName);
        }

        if (linkedLaser != null)
        {
            linkedLaser.StartGrowing();
        }
    }

    void ChangeLightColor()
    {
        if (targetLight != null)
        {
            targetLight.color = powerOnColor;
        }
    }

    void PlayPowerSound(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + fileName);

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Audio not found! Resources/Sounds name: " + fileName);
        }
    }

    void TogglePower(bool state)
    {
        if (allLightsParent != null)
        {
            allLightsParent.SetActive(state);

            Debug.Log("<color=yellow>Power Status: " + state + "</color>");
        }
    }

    void RotateHandle()
    {
        if (leverHandle != null)
        {
            Vector3 currentRot = leverHandle.localEulerAngles;

            leverHandle.localEulerAngles =
                new Vector3(80f, currentRot.y, currentRot.z);
        }
    }

    void EnableTargetCollider()
    {
        if (colliderToEnable != null)
        {
            colliderToEnable.enabled = true;

            Debug.Log("Success! Collider: " +
                      colliderToEnable.gameObject.name +
                      " is now active.");
        }
    }

    void SaveState()
    {
        PlayerPrefs.SetInt(saveID + "_Power", 1);
        PlayerPrefs.Save();
    }

void LoadState()
{
    if (string.IsNullOrEmpty(saveID)) return;

    if (PlayerPrefs.HasKey(saveID + "_Power"))
    {
        isActivated = true;

        if (allLightsParent != null)
            allLightsParent.SetActive(true);

        RotateHandle();
        EnableTargetCollider();
        ChangeLightColor();

        if (targetAnimator != null)
            targetAnimator.Play(animationToPlay, 0, 1f);

        if (linkedLaser != null)
            linkedLaser.StartGrowing();
    }
    else
    {
        if (allLightsParent != null)
            allLightsParent.SetActive(false);
    }
}
}