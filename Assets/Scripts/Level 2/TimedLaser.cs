using UnityEngine;
using System.Collections;

public class TimedLaser : MonoBehaviour
{
    public GameObject laserObject;
    public float toggleInterval = 3.0f;

    void Start()
    {
        StartCoroutine(ToggleLaserRoutine());
    }

    IEnumerator ToggleLaserRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(toggleInterval);
            
            if (laserObject != null)
            {
                laserObject.SetActive(!laserObject.activeSelf);
            }
        }
    }
}