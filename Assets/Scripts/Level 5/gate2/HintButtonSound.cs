using UnityEngine;
using System.Collections;

public class HintButtonSound : MonoBehaviour, IInteractable
{
    public AudioSource audioSource;
    public AudioClip hintSound;

    [Range(0f, 1f)]
    public float volume = 0.1f;

    public void Interact()
    {
        if (audioSource == null || hintSound == null) return;

        audioSource.PlayOneShot(hintSound, volume);
    }
}