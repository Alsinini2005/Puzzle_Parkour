using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Animator doorAnimator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";

    private int objectsOnPlate = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            objectsOnPlate++;

            if (objectsOnPlate == 1)
            {
                doorAnimator.SetTrigger(openTrigger);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            objectsOnPlate--;

            if (objectsOnPlate <= 0)
            {
                objectsOnPlate = 0;
                doorAnimator.SetTrigger(closeTrigger);
            }
        }
    }
}