using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    private List<GhostFrame> frames = new List<GhostFrame>();
    private bool isRecording;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        StartRecording();
    }

    public void StartRecording()
    {
        frames.Clear();
        isRecording = true;
    }

    void FixedUpdate()
{
    if (!isRecording)
        return;

    frames.Add(new GhostFrame
    {
        position = transform.position,
        rotation = transform.rotation,

        isWalking        = anim != null && anim.GetBool("isWalking"),
        isRunning        = anim != null && anim.GetBool("isRunning"),
        isGrounded       = anim != null && anim.GetBool("isGrounded"),
        isCrouching      = anim != null && anim.GetBool("isCrouching"),
        isCrouchingWalking = anim != null && anim.GetBool("isCrouchingWalking"),
        jumpTrigger      = anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("Jumping"),
    });
}

    public List<GhostFrame> StopAndGetRecording()
    {
        isRecording = false;
        return new List<GhostFrame>(frames);
    }

    public void SaveToGhostManager(){
    isRecording = false;
    int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    GhostManager.instance?.SetGhostData(frames, currentLevel);
}
}