using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Correct Order")]
    public int[] correctOrder = { 2, 1, 3 };
    [Header("Light Save")]
    public bool saveLightsState = true;
    private List<int> input = new List<int>();
    private bool completed = false;

    [Header("Buttons")]
    public PuzzleButton[] buttons;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Win Objects")]
    public GameObject objectToEnable;
    public GameObject objectToDisable;

    [Header("Lights")]
    public Light[] lightsToChange;
    public Color winColor = Color.blue;

    [Header("Save System")]
    public string saveID;

    void Start()
    {
        LoadState();
    }

    public void PressButton(int id, bool state)
    {
        if (completed) return;

        PlaySound();

        input.Add(id);

        if (input.Count == correctOrder.Length)
        {
            CheckResult();
        }
    }

    void CheckResult()
    {
        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (input[i] != correctOrder[i])
            {
                ShowWrong();
                ResetPuzzle();
                return;
            }
        }

        Win();
    }

    void ShowWrong()
    {
        if (LevelIntroController.Instance != null)
        {
            LevelIntroController.Instance.ShowIntro(
                "[note]",
                "Wrong order! Try again."
            );
        }
    }

    void ResetPuzzle()
    {
        input.Clear();

        foreach (var b in buttons)
        {
            if (b != null)
                b.ForceOff();
        }
    }
public void ResetState()
{
    if (!string.IsNullOrEmpty(saveID))
    {
        PlayerPrefs.DeleteKey(saveID + "_Solved");
        PlayerPrefs.DeleteKey(saveID + "_Lights");
        for (int i = 0; i < lightsToChange.Length; i++)
        {
            PlayerPrefs.DeleteKey(saveID + "_Light_" + i + "_Enabled");
            PlayerPrefs.DeleteKey(saveID + "_LightR_" + i);
            PlayerPrefs.DeleteKey(saveID + "_LightG_" + i);
            PlayerPrefs.DeleteKey(saveID + "_LightB_" + i);
        }
        PlayerPrefs.Save();
    }

    completed = false;
    input.Clear();

    if (objectToEnable != null) objectToEnable.SetActive(false);
    if (objectToDisable != null) objectToDisable.SetActive(true);

    foreach (var b in buttons)
        if (b != null) b.ForceOff();

    foreach (var l in lightsToChange)
        if (l != null) l.color = Color.white;
}
    void Win()
    {
        completed = true;
        LevelIntroController.Instance.ShowIntro(
                "[Win]",
                "PUZZLE *COMPLETE!*"
            );
        Debug.Log("PUZZLE COMPLETE!");

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        foreach (var l in lightsToChange)
        {
            if (l != null)
                l.color = winColor;
        }

        SaveState();
    }

    void PlaySound()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound, 0.3f);
    }

    // ======================
    // SAVE SYSTEM
    // ======================

    void SaveState()
    {
        if (string.IsNullOrEmpty(saveID)) return;

        PlayerPrefs.SetInt(saveID + "_Solved", 1);

        if (saveLightsState && lightsToChange != null)
        {
            PlayerPrefs.SetInt(saveID + "_Lights", 1);

            for (int i = 0; i < lightsToChange.Length; i++)
            {
                if (lightsToChange[i] != null)
                {
                    PlayerPrefs.SetInt(saveID + "_Light_" + i + "_Enabled", lightsToChange[i].enabled ? 1 : 0);

                    PlayerPrefs.SetFloat(saveID + "_LightR_" + i, lightsToChange[i].color.r);
                    PlayerPrefs.SetFloat(saveID + "_LightG_" + i, lightsToChange[i].color.g);
                    PlayerPrefs.SetFloat(saveID + "_LightB_" + i, lightsToChange[i].color.b);
                }
            }
        }

        PlayerPrefs.Save();
    }

    void LoadState()
    {
        if (string.IsNullOrEmpty(saveID)) return;

        bool solved = PlayerPrefs.GetInt(saveID + "_Solved", 0) == 1;

        if (solved)
        {
            completed = true;

            if (objectToEnable != null)
                objectToEnable.SetActive(true);

            if (objectToDisable != null)
                objectToDisable.SetActive(false);
        }

        if (solved)
    {
        foreach (var l in lightsToChange)
        {
            if (l != null)
            {
                l.enabled = true;
                l.color = winColor;
            }
        }
    }
    }
}