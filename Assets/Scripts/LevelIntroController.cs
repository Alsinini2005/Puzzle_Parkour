using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelIntroController : MonoBehaviour
{
    public static LevelIntroController Instance { get; private set; }

    private CanvasGroup introGroup;
    private CanvasGroup overlayGroup;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI subtitleText;
    private Image accentLine;
    private RectTransform overlayRect;

    [Header("Settings")]
    public float fadeInTime = 1.0f;
    public float waitTime = 5.0f;
    public float fadeOutTime = 1.0f;
    public string keyColor = "#FF4D00";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        FindReferences();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => FindReferences();

    private void FindReferences()
    {
        GameObject container = GameObject.Find("Intro_MainContainer");
        if (container != null)
        {
            introGroup = container.GetComponent<CanvasGroup>();
            overlayGroup = container.transform.Find("LevelIntroOverlay").GetComponent<CanvasGroup>();
            overlayRect = container.transform.Find("LevelIntroOverlay").GetComponent<RectTransform>();

            Transform ui = container.transform.Find("LevelIntroOverlay/LevelIntroUI");
            if (ui != null)
            {
                titleText = ui.Find("LevelTitle_LevelIntro").GetComponent<TextMeshProUGUI>();
                subtitleText = ui.Find("LevelSubtitle_LevelIntro").GetComponent<TextMeshProUGUI>();
                accentLine = ui.Find("AccentLine_LevelIntro").GetComponent<Image>();
            }
        }
    }

    public void ShowIntro(string levelName, params string[] lines)
    {
        if (titleText == null) FindReferences();

        StopAllCoroutines();

        if (introGroup != null) introGroup.gameObject.SetActive(true);
        if (overlayGroup != null) overlayGroup.gameObject.SetActive(true);

        if (titleText != null) titleText.text = levelName;

        string combinedText = "";
        for (int i = 0; i < lines.Length; i++)
        {
            combinedText += FormatText(lines[i]) + (i < lines.Length - 1 ? "\n" : "");
        }

        if (subtitleText != null) subtitleText.text = combinedText;

        StartCoroutine(RefreshAndStart());
    }

    IEnumerator RefreshAndStart()
    {
        SetAlphas(0);
        yield return new WaitForEndOfFrame();

        UpdateLayout();
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            SetAlphas(timer / fadeInTime);
            yield return null;
        }
        SetAlphas(1);

        yield return new WaitForSeconds(waitTime);

        timer = 0;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            SetAlphas(1 - (timer / fadeOutTime));
            yield return null;
        }
        SetAlphas(0);

        if (introGroup != null) introGroup.gameObject.SetActive(false);
        if (overlayGroup != null) overlayGroup.gameObject.SetActive(false);
    }

    void SetAlphas(float alpha)
    {
        if (introGroup != null) introGroup.alpha = alpha;
        if (overlayGroup != null) overlayGroup.alpha = alpha;
    }

    void UpdateLayout()
    {
        if (overlayRect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(overlayRect);

        if (accentLine != null && titleText != null)
        {
            float titleHeight = titleText.preferredHeight;
            accentLine.rectTransform.anchoredPosition = new Vector2(0, -titleHeight / 2 - 15f);
        }
    }

    string FormatText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        string formatted = text;
        int count = 0;
        while (formatted.Contains("*"))
        {
            string tag = (count % 2 == 0) ? "<color=" + keyColor + ">" : "</color>";
            formatted = ReplaceFirst(formatted, "*", tag);
            count++;
        }
        return formatted;
    }

    string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0) return text;
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }
}