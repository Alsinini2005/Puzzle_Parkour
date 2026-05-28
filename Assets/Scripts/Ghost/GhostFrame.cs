using UnityEngine;

[System.Serializable]
public struct GhostFrame
{
    public Vector3 position;
    public Quaternion rotation;

    // Animation states
    public bool isWalking;
    public bool isRunning;
    public bool isGrounded;
    public bool isCrouching;
    public bool isCrouchingWalking;
    public bool jumpTrigger;
}