using UnityEngine;
using UnityEngine.InputSystem;

public class FPSMapInspect : MonoBehaviour
{
    [Header("UI Map Images")]
    public GameObject smallHandMap;  
    public GameObject largeCenterMap; 

    private bool isViewingLargeMap = false;
    private bool isSystemActive = false; 
    private bool skipFrame = false; 

    void Start()
    {
        if (largeCenterMap != null) largeCenterMap.SetActive(false);
        if (smallHandMap != null) smallHandMap.SetActive(false);
        isSystemActive = false;
    }

    void Update()
    {
        if (!isSystemActive) return; 

        if (skipFrame)
        {
            skipFrame = false;
            return;
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.mKey.wasPressedThisFrame)
            {
                ToggleMaps();
            }
        }
    }

    public void OpenMapOnPickup()
    {
        isSystemActive = true;
        skipFrame = true; 
        ShowSmallMap();

        if (LevelIntroController.Instance != null)
        {
            LevelIntroController.Instance.ShowIntro("[note]", "Press *[E]* or *[M]* to toggle map zoom!");
        }
    }

    private void ToggleMaps()
    {
        if (isViewingLargeMap)
        {
            ShowSmallMap();
        }
        else
        {
            ShowLargeMap();
        }
    }

    private void ShowLargeMap()
    {
        isViewingLargeMap = true;
        if (largeCenterMap != null) largeCenterMap.SetActive(true);  
        if (smallHandMap != null) smallHandMap.SetActive(false);    
    }

    private void ShowSmallMap()
    {
        isViewingLargeMap = false;
        if (smallHandMap != null) smallHandMap.SetActive(true);     
        if (largeCenterMap != null) largeCenterMap.SetActive(false); 
    }

    public void ForceHideMapOnDeath()
    {
    isSystemActive = false;
    isViewingLargeMap = false;
    
    if (smallHandMap != null) smallHandMap.SetActive(false);
    if (largeCenterMap != null) largeCenterMap.SetActive(false);
    }
}