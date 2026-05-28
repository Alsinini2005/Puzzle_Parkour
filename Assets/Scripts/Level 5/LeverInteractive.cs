using UnityEngine;
using System.Collections.Generic;

public class LeverInteractive : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public int leverID; 
    public GameObject successObject; 

    [Header("Lava Controllers")]
    public List<GameObject> lavaGameObjects; 

    public void Interact()
    {
        float chance = Random.value; 

        if (successObject != null)
            successObject.SetActive(true);

        foreach (var lavaObj in lavaGameObjects)
        {
            if (lavaObj != null)
            {
                lavaObj.SetActive(true); 

                var lavaScript = lavaObj.GetComponent<LavaRiser>();
                if (lavaScript != null)
                {
                    if (chance < 0.2f)
                    {
                        lavaScript.StopLavaRise();
                        LevelIntroController.Instance.ShowIntro(
                        "[Lucky!]", 
                        "YOU DODGED THE LAVA! *LUCKY YOU!*"
                    );
                    }
                    else
                    {
                        lavaScript.StartLavaRise();
                        LevelIntroController.Instance.ShowIntro(
                        "[Danger!]", 
                        "LAVA ACTIVATED! *RUN FOR YOUR LIFE!*"
                    );
                    }
                }
            }
        }
    }
}