using UnityEngine;
using System.Collections.Generic;

public class LevelCompletionTrigger : MonoBehaviour
{
    [Header("Requirements")]
    public List<GameObject> electricalObjects;
    public List<GameObject> gatesToDeactivate;
    
    public int levelNumber = 6;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successClip;

    [Header("Animation")]
    public Animator levelAnimator;
    public string animationTrigger = "StartLevel6";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckConditions())
            {
                PlaySuccessSound();
                TriggerLevelAnimation();
                
            }
        }
    }

    bool CheckConditions()
    {
        foreach (GameObject obj in electricalObjects)
        {
            if (obj == null || !obj.activeSelf) return false;
        }

        foreach (GameObject gate in gatesToDeactivate)
        {
            if (gate != null && gate.activeSelf) return false;
        }

        return true;
    }

    void PlaySuccessSound()
    {
        if (audioSource != null && successClip != null)
        {
            audioSource.PlayOneShot(successClip);
        }
    }

    void TriggerLevelAnimation()
    {
        if (levelAnimator != null)
        {
            levelAnimator.SetTrigger(animationTrigger);
        }
    }
}