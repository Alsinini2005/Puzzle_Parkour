using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button continueButton;

    [Header("Main Menu Panel")]
    public GameObject mainMenuPanel;

    [Header("UI Panels")]
    public GameObject instructionsPanel;
    public GameObject creditsPanel;
    public GameObject settingsPanel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (continueButton != null)
        {
            continueButton.interactable = PlayerPrefs.HasKey("LastLevel");
        }
    }

    public void OpenPanel(GameObject panelToOpen)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (panelToOpen != null) panelToOpen.SetActive(true);
    }

    public void ClosePanel(GameObject panelToClose)
    {
        if (panelToClose != null) panelToClose.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void OpenInstructions() => OpenPanel(instructionsPanel);
    public void OpenCredits() => OpenPanel(creditsPanel);
    public void OpenSettings() => OpenPanel(settingsPanel);

    public void CloseInstructions() => ClosePanel(instructionsPanel);
    public void CloseCredits() => ClosePanel(creditsPanel);
    public void CloseSettings() => ClosePanel(settingsPanel);

    public void StartGame()
    {
        int lastScene = PlayerPrefs.GetInt("LastLevel", 1);
        SceneManager.LoadScene(lastScene);
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(1);
    }

    public void QuitGame() => Application.Quit();
}