using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider volSlider;
    public Slider musicSlider;
    public Toggle fullToggle;
    public Slider sensSlider;
    public GameObject settingsPanel;
    [Header("Audio Settings")]
    public AudioMixer mainAudioMixer;

    void Start()
    {
        LoadSettings();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                OpenSettings();
            }
        }
    }

    public void ApplySettings()
    {
        PlayerPrefs.SetFloat("Volume", volSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetInt("Fullscreen", fullToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("Sensitivity", sensSlider.value);
        PlayerPrefs.Save();

        ApplyValuesOnly();
        CloseSettings(); 
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void LoadSettings()
    {
        volSlider.value = PlayerPrefs.GetFloat("Volume", 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        fullToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        sensSlider.value = PlayerPrefs.GetFloat("Sensitivity", 5.0f);
        
        ApplyValuesOnly();
    }

    private void ApplyValuesOnly()
    {
        float volume = Mathf.Clamp(volSlider.value, 0.0001f, 1f);
        mainAudioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        float musicVol = Mathf.Clamp(musicSlider.value, 0.0001f, 1f);
        mainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVol) * 20);

        Screen.fullScreen = fullToggle.isOn;
    }

    public void ResetToDefaults()
    {
        volSlider.value = 0.75f;
        musicSlider.value = 0.75f;
        fullToggle.isOn = true;
        sensSlider.value = 5.0f;
        ApplySettings();
    }
}