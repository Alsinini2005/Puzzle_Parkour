using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnvironmentActivator : MonoBehaviour, IInteractable
{
    [Header("1. Activation Settings")]
    public GameObject objectToEnable;
    public GameObject objectToDisable;

    [Header("2. Light Settings")]
    public List<Light> lightsToChangeColor;
    public Color newLightColor = Color.blue;

    [Header("3. Rotation Settings (The Lever)")]
    public Transform leverHandle;
    public float targetRotationX = 80f;

    [Header("4. Save System")]
    public string saveID;
    private bool isActivated = false;
    [Header("Animation Settings")]
    public Animator leverAnimator;
    public string animationTrigger = "Activate";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip interactClip;
    [Header("Light Pulse Effect")]
    public float pulseDelay = 0.3f;
    public int pulseCount = 3;
    [Header("External Trigger")]
    public Collider activationTriggerCollider;
    void Start()
    {
        LoadState();
    }

    public void Interact()
    {
        if (isActivated) return;
        TryActivate();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryActivate();
        }
    }
    void TryActivate()
    {
        if (isActivated) return;

        ActivateSystem();
    }
    
    void ActivateSystem()
    {
        isActivated = true;

        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger(animationTrigger);
        }
        else
        {
            RotateLever();
        }

        PlayInteractSound();

        if (objectToEnable != null) objectToEnable.SetActive(true);
        if (objectToDisable != null) objectToDisable.SetActive(false);

        SaveState();

        StartCoroutine(DelayedPulse());
    }
    IEnumerator DelayedPulse()
    {
        yield return new WaitForSeconds(5f);

        StartCoroutine(LightPulseRoutine());
    }
    IEnumerator LightPulseRoutine()
    {
        for (int i = 0; i < pulseCount; i++)
        {
            bool isOn = (i % 2 == 0);

            foreach (Light light in lightsToChangeColor)
            {
                if (light != null)
                    light.enabled = isOn;
            }

            PlayInteractSound();

            yield return new WaitForSeconds(pulseDelay);
        }

        foreach (Light light in lightsToChangeColor)
        {
            if (light != null)
            {
                light.enabled = true;
                light.color = newLightColor;
            }
        }
    }

void PlayInteractSound()
{
    if (audioSource == null) return;

    if (interactClip == null)
    {
        interactClip = Resources.Load<AudioClip>("Sounds/open-doors-114615");
    }

    if (interactClip != null)
    {
        audioSource.PlayOneShot(interactClip, 0.3f);
    }
}

    void RotateLever()
    {
        if (leverHandle != null)
        {
            Vector3 currentRot = leverHandle.localEulerAngles;
            leverHandle.localEulerAngles = new Vector3(targetRotationX, currentRot.y, currentRot.z);
        }
    }

    void SaveState()
    {
        if (!string.IsNullOrEmpty(saveID))
        {
            PlayerPrefs.SetInt(saveID + "_Active", 1);
            PlayerPrefs.Save();
        }
    }

    void LoadState()
    {
        if (string.IsNullOrEmpty(saveID)) return;

        if (PlayerPrefs.GetInt(saveID + "_Active", 0) == 1)
        {
            isActivated = true;

            if (objectToEnable != null)
                objectToEnable.SetActive(true);

            if (objectToDisable != null)
                objectToDisable.SetActive(false);

            RotateLever();

            foreach (Light light in lightsToChangeColor)
            {
                if (light != null)
                {
                    light.enabled = true;
                    light.color = newLightColor;
                }
            }
        }
    }

    public void ResetState()
{
    if (!string.IsNullOrEmpty(saveID))
    {
        PlayerPrefs.DeleteKey(saveID + "_Active");
        PlayerPrefs.Save();
    }

    isActivated = false;

    if (objectToEnable != null) objectToEnable.SetActive(false);
    if (objectToDisable != null) objectToDisable.SetActive(true);

    if (leverHandle != null)
    {
        Vector3 rot = leverHandle.localEulerAngles;
        leverHandle.localEulerAngles = new Vector3(0f, rot.y, rot.z);
    }

    foreach (Light light in lightsToChangeColor)
    {
        if (light != null)
        {
            light.enabled = true;
            light.color = Color.white;
        }
    }
}
}