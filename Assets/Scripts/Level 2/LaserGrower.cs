using UnityEngine;
using System.Collections;
using VolumetricLines; 
using UnityEngine.Audio;

public class LaserGrower : MonoBehaviour
{
    public float targetY = 35.2f;
    public float duration = 2.0f;
    private string beamSoundName = "futuristic-beam-81215";
    [Range(0f, 1f)]
    private float soundStartTimePercent = 0.5f;

    private VolumetricLineBehavior lineScript;
    private AudioSource beamAudioSource;
    [Header("Audio Mixer")]
    public AudioMixerGroup outputMixerGroup;
    void Awake()
    {
        beamAudioSource = gameObject.AddComponent<AudioSource>();
        beamAudioSource.playOnAwake = false;
        beamAudioSource.spatialBlend = 1f;
        beamAudioSource.loop = true;
        if (outputMixerGroup != null)
        {
            beamAudioSource.outputAudioMixerGroup = outputMixerGroup;
        }
    }

    void Start()
    {
        lineScript = GetComponent<VolumetricLineBehavior>();
    }

    public void StartGrowing()
    {
        if (lineScript != null)
        {
            StartCoroutine(AnimateLaser());
        }
    }

    IEnumerator AnimateLaser()
    {
        float elapsed = 0;
        Vector3 startPos = lineScript.StartPos;
        float initialY = startPos.y;

        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + beamSoundName);
        if (clip != null)
        {
            beamAudioSource.clip = clip;
            beamAudioSource.time = clip.length * soundStartTimePercent; 
            beamAudioSource.Play();
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentY = Mathf.Lerp(initialY, targetY, elapsed / duration);
            
            Vector3 newPos = lineScript.StartPos;
            newPos.y = currentY;
            lineScript.StartPos = newPos;

            yield return null;
        }

        Vector3 finalPos = lineScript.StartPos;
        finalPos.y = targetY;
        lineScript.StartPos = finalPos;

        if (beamAudioSource.isPlaying)
        {
            beamAudioSource.Stop();
        }
    }
}