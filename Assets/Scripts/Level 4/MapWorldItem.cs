using UnityEngine;

public class MapWorldItem : MonoBehaviour, IInteractable
{
    [Header("References")]
    public FPSMapInspect mapSystemScript;

    public void Interact()
    {
        if (mapSystemScript != null)
        {
            mapSystemScript.OpenMapOnPickup();
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("FPSMApInspect is not pulled in the Inspector!");
        }
    }
}