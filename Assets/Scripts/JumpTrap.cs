using UnityEngine;

public class JumpTrap : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float bounceForce = 25f;
    [Header("Sound Settings")]
    [SerializeField] private AudioClip jumpSoundClip; 

    [Header("Animation Settings")]
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            if (player != null)
            {
                player.BouncePlayer(bounceForce);
            }

            if (jumpSoundClip != null)
            {
                AudioSource.PlayClipAtPoint(jumpSoundClip, transform.position);
            }

            if (animator != null)
            {
                animator.SetTrigger("Jump"); 
            }
        }
    }
}