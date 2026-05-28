using System.Collections;
using UnityEngine;

public class LaserTriggerZone : MonoBehaviour
{
    [Header("Laser Settings")]
    public LaserGrower laserToActivate;

    [Header("Animation Settings")]
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private string animationStateName;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip laserEndClip;
    [SerializeField] private AudioSource targetAudioSource;

    [Header("Delay Settings")]
    [SerializeField] private float delayTime = 2.0f;

    [Header("Save Settings")]
    public string zoneUniqueSaveKey;

    private bool hasTriggered = false;

    void Start()
    {
        if (GameManager.instance != null && !string.IsNullOrEmpty(zoneUniqueSaveKey))
        {
            if (GameManager.instance.LoadTriggerEvent(zoneUniqueSaveKey))
            {
                hasTriggered = true;
                if (laserToActivate != null)
                {
                    laserToActivate.StartGrowing();
                }
                if (targetAnimator != null && !string.IsNullOrEmpty(animationStateName))
                {
                    targetAnimator.enabled = true;
                    targetAnimator.Play(animationStateName, 0, 1f);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (laserToActivate != null)
            {
                hasTriggered = true;
                laserToActivate.StartGrowing(); 

                if (GameManager.instance != null && !string.IsNullOrEmpty(zoneUniqueSaveKey))
                {
                    GameManager.instance.SaveTriggerEvent(zoneUniqueSaveKey, true);
                }

                if (targetAnimator != null)
                {
                    targetAnimator.gameObject.AddComponent<TempCoroutineRunner>().StartStaticCoroutine(PlayEffectsWithDelay());
                }
            }
            else
            {
                Debug.LogWarning("Link Laser in Inspector!");
            }
        }
    }

    private IEnumerator PlayEffectsWithDelay()
    {
        yield return new WaitForSeconds(delayTime);

        if (targetAnimator != null && !string.IsNullOrEmpty(animationStateName))
        {
            targetAnimator.enabled = true;
            targetAnimator.Play(animationStateName, 0, 0f);
        }

        if (laserEndClip != null && targetAudioSource != null)
        {
            targetAudioSource.clip = laserEndClip;
            targetAudioSource.Play();
        }
    }
}