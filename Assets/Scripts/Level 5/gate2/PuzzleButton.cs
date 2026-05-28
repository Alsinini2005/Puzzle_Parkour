using UnityEngine;

public class PuzzleButton : MonoBehaviour, IInteractable
{
    public int buttonID;
    public PuzzleManager manager;

    public Animator anim;

    private bool isOn = false;

    public void Interact()
    {
        ToggleState();

        if (manager != null)
            manager.PressButton(buttonID, isOn);
    }

    void ToggleState()
    {
        isOn = !isOn;

        if (anim != null)
        {
            anim.SetBool("isOn", isOn);
        }
    }

    public void ForceOff()
    {
        isOn = false;

        if (anim != null)
        {
            anim.SetBool("isOn", false);
        }
    }

    public bool IsOn()
    {
        return isOn;
    }
}