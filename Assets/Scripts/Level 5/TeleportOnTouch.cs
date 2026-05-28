using UnityEngine;
using System.Collections;

public class TeleportOnTouch : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform targetPosition;

    [Header("Optional Settings")]
    public bool resetVelocity = true;

    [Header("Gate Number")]
    public int gateNumber = 0;

    [Header("Lava Settings")]
    public LavaRiser lava1;
    public LavaRiser lava2;

private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        if (gateNumber > 0)
        {
            PlayerPrefs.SetInt("LastGateReached", gateNumber);
            PlayerPrefs.Save();
            Debug.Log("Progress has been saved at Gate Number: " + gateNumber);
        }

        if (gateNumber == 1)
        {
            StartCoroutine(ActivateLavaAfterDelay());
            if (LevelIntroController.Instance != null)
            {
                LevelIntroController.Instance.ShowIntro("[WARNING]", "*RUN FAST!!*");
            }
        }

        TeleportPlayer(other.gameObject);
    }
}

    IEnumerator ActivateLavaAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (lava1 != null)
            lava1.isRising = true;

        if (lava2 != null)
            lava2.isRising = true;
    }

    private void TeleportPlayer(GameObject player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.transform.position = targetPosition.position;
        player.transform.rotation = targetPosition.rotation;

        if (resetVelocity)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (cc != null)
            cc.enabled = true;
    }
}