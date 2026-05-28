using UnityEngine;

public class LavaRiser : MonoBehaviour
{
    [Header("Movement Settings")]
    public float riseSpeed = 0.8f;
    public float maxHeight = 100f;
    [Header("Activation")]
    public bool isRising = true;

    private Vector3 originalPosition;

    void Awake()
    {
        originalPosition = transform.position;
    }

void Update()
    {
        if (isRising)
        {
            transform.Translate(Vector3.up * riseSpeed * Time.deltaTime, Space.World);
            
            if (riseSpeed < 0 && transform.position.y < originalPosition.y)
            {
                transform.position = originalPosition;
                isRising = false;
            }
        }
    }

    public void StartLavaRise() => isRising = true;
    public void StopLavaRise() => isRising = false;

    public void ResetToOriginalPosition()
    {
        transform.position = originalPosition;
        isRising = true;
        riseSpeed = Mathf.Abs(riseSpeed);
    }
}