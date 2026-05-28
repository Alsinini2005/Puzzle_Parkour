using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightSequenceController : MonoBehaviour, IInteractable
{
    [Header("Animation")]
    public Animator animator;
    public string animationTriggerName = "Play";
    
    [Header("Dependencies")]
    public InteractionTrigger mainPowerSource;

    [Header("Lights Sequence")]
    public List<GameObject> sequentialLights;
    public float delayBetweenLights = 1f;

    [Header("Audio")]
    private AudioSource audioSource;
    public string switchSoundName = "Light-Switch-Flip";

    [Header("Save Settings")]
    public string lightSequenceUniqueSaveKey;

    private bool sequencePlayed = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void Start()
    {
        if (GameManager.instance != null && !string.IsNullOrEmpty(lightSequenceUniqueSaveKey))
        {
            if (GameManager.instance.LoadTriggerEvent(lightSequenceUniqueSaveKey))
            {
                ApplySavedLightsState();
                return; 
            }
        }

        SetAllLightsActive(false);
    }

    private void ApplySavedLightsState()
    {
        sequencePlayed = true;

        if (mainPowerSource != null && mainPowerSource.allLightsParent != null)
        {
            mainPowerSource.allLightsParent.SetActive(true);
        }

        SetAllLightsActive(true);
    }

    private void SetAllLightsActive(bool state)
    {
        foreach (GameObject lightObj in sequentialLights)
        {
            if (lightObj != null) 
            {
                lightObj.SetActive(state);
            }
        }
    }

    public void Interact()
    {
        if (sequencePlayed) return;

        ResetAndPlaySequence();
    }

    public void ResetAndPlaySequence()
    {
        if (mainPowerSource != null && mainPowerSource.allLightsParent.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(PlayLightSequence());
        }
        else
        {
            Debug.Log("Power is not active yet!");
        }
    }

    IEnumerator PlayLightSequence()
    {
        sequencePlayed = true;

        if (GameManager.instance != null && !string.IsNullOrEmpty(lightSequenceUniqueSaveKey))
        {
            GameManager.instance.SaveTriggerEvent(lightSequenceUniqueSaveKey, true);
        }

        if (animator != null)
        {
            animator.SetTrigger(animationTriggerName);
        }

        SetAllLightsActive(false);

        foreach (GameObject lightObj in sequentialLights)
        {
            if (lightObj != null)
            {
                lightObj.SetActive(true);
                PlaySwitchSound();
                yield return new WaitForSeconds(delayBetweenLights);
            }
        }
    }

    void PlaySwitchSound()
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + switchSoundName);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sequencePlayed)
        {
            Interact();
        }
    }
}