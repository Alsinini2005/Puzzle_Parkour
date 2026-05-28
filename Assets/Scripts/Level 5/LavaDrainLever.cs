using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LavaDrainLever : MonoBehaviour, IInteractable
{
    [Header("Lava Settings")]
    public List<LavaRiser> lavaRisers;
    public float drainSpeed = -2f;

    [Header("Gate Settings")]
    public GameObject gateToOpen;
    public GameObject gateToClose;

    [Header("Light Settings")]
    public List<Light> lightsToChange;
    public Color lightColor = Color.blue;

    [Header("Save Settings")]
    public string saveID;
    private bool isActivated = false;
    [Header("Animation Settings")]
    public Animator leverAnimator;
    public string animationTrigger = "Activate";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip interactClip;
    void Start()
    {
        LoadState();
    }

    public void Interact()
    {
        if (isActivated) return;
        ActivateDrain();
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
void ActivateDrain()
{
    isActivated = true;

    PlayInteractSound();
    if (leverAnimator != null)
        {
            leverAnimator.SetTrigger(animationTrigger);
        }

    foreach (var lava in lavaRisers)
    {
        if (lava != null)
        {
            lava.StopLavaRise();
        }
    }

    if (gateToOpen != null) gateToOpen.SetActive(true);
    if (gateToClose != null) gateToClose.SetActive(false);
    foreach (Light light in lightsToChange)
    {
        if (light != null) light.color = lightColor;
    }

    StartCoroutine(StartLavaAfterDelay(10f));
    SaveState();
}

IEnumerator StartLavaAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    
    foreach (var lava in lavaRisers)
    {
        if (lava != null)
        {
            lava.riseSpeed = drainSpeed;
            lava.StartLavaRise();
        }
    }

    StartCoroutine(DisableLavaAfterDelay(7f));
}

    IEnumerator DisableLavaAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var lava in lavaRisers)
        {
            if (lava != null) lava.gameObject.SetActive(false);
        }
    }

    void SaveState()
    {
        if (!string.IsNullOrEmpty(saveID))
        {
            PlayerPrefs.SetInt(saveID + "_Drained", 1);
            PlayerPrefs.Save();
        }
    }

    void LoadState()
    {
        if (string.IsNullOrEmpty(saveID)) return;

        if (PlayerPrefs.GetInt(saveID + "_Drained", 0) == 1)
        {
            isActivated = true;
            if (gateToOpen != null) gateToOpen.SetActive(true);
            if (gateToClose != null) gateToClose.SetActive(false);
            
            foreach (var lava in lavaRisers)
            {
                if (lava != null) lava.gameObject.SetActive(false);
            }

            foreach (Light light in lightsToChange)
            {
                if (light != null) light.color = lightColor;
            }
        }
    }

public void ResetState()
{
    if (!string.IsNullOrEmpty(saveID))
    {
        PlayerPrefs.DeleteKey(saveID + "_Drained");
        PlayerPrefs.Save();
    }

    isActivated = false;

    foreach (var lava in lavaRisers)
    {
        if (lava != null)
        {
            lava.gameObject.SetActive(true);
            lava.ResetToOriginalPosition();
        }
    }

    if (gateToOpen != null) gateToOpen.SetActive(false);
    if (gateToClose != null) gateToClose.SetActive(true);

    foreach (Light light in lightsToChange)
    {
        if (light != null) light.color = Color.white;
    }
}
}