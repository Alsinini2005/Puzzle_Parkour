using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 movementOffset;
    public float speed = 2f;
    public float waitTime = 0.5f;

    private Vector3 startPos;
    private Vector3 finalPos;
    private Vector3 nextPos;
    private float waitTimer;
    private bool isWaiting;
    
    public Vector3 DeltaPosition { get; private set; }
    private Vector3 lastPosition;

    void Start()
    {
        startPos = transform.position;
        finalPos = startPos + movementOffset;
        nextPos = finalPos;
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (isWaiting)
        {
            DeltaPosition = Vector3.zero;
            waitTimer += Time.fixedDeltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            lastPosition = transform.position;
            return;
        }

        MovePlatform();
    }

    void MovePlatform()
    {
        Vector3 targetPos = Vector3.MoveTowards(transform.position, nextPos, speed * Time.fixedDeltaTime);
        
        transform.position = targetPos;
        
        DeltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;

        if (Vector3.Distance(transform.position, nextPos) < 0.0001f)
        {
            isWaiting = true;
            nextPos = (nextPos == startPos) ? finalPos : startPos;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 visualStart = Application.isPlaying ? startPos : transform.position;
        Vector3 visualEnd = visualStart + movementOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(visualStart, visualEnd);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(visualStart, transform.localScale);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(visualEnd, transform.localScale);
    }
}