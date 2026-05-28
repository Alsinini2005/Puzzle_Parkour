using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    public List<GhostFrame> framesToPlay;
    private int index;
    private bool isPlaying = false;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (framesToPlay != null && framesToPlay.Count > 0)
        {
            isPlaying = true;
            
        }
    }

    void FixedUpdate()
{
    if (!isPlaying)
        return;

    if (framesToPlay == null || framesToPlay.Count == 0)
        return;

    if (index >= framesToPlay.Count)
    {
        isPlaying = false;
        return;
    }

    GhostFrame frame = framesToPlay[index];

    transform.position = frame.position;
    transform.rotation = frame.rotation;

    if (anim != null)
        {
            anim.SetBool("isWalking",         frame.isWalking);
            anim.SetBool("isRunning",         frame.isRunning);
            anim.SetBool("isGrounded",        frame.isGrounded);
            anim.SetBool("isCrouching",       frame.isCrouching);
            anim.SetBool("isCrouchingWalking",frame.isCrouchingWalking);
            
            if (frame.jumpTrigger)
                anim.SetTrigger("JumpTrigger");
        }
        
    index++;
}

    public void ResetPlayback()
{
    index = 0;

    if (framesToPlay != null && framesToPlay.Count > 0)
    {
        transform.position = framesToPlay[0].position;
        transform.rotation = framesToPlay[0].rotation;

        isPlaying = true;
    }
    else
    {
        isPlaying = false;
    }
}
}