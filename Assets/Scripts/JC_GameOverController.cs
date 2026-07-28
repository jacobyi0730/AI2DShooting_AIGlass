using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JC_GameOverController : MonoBehaviour
{
    [SerializeField] private JC_Health health;

    private GameObject _gameOverPanel;
    private bool _isGameOver;

    private void Awake()
    {
        if (health == null)
        {
            health = GetComponent<JC_Health>();
        }

        CreateGameOverUi();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.Died += ShowGameOver;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Died -= ShowGameOver;
        }
    }

    private void ShowGameOver()
    {
        if (_isGameOver)
        {
            return;
        }

        _isGameOver = true;
        Time.timeScale = 0f;
        _gameOverPanel.SetActive(true);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void CreateGameOverUi()
    {
        GameObject canvasObject = new GameObject("GameOverCanvas");

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(768f, 1024f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        EnsureLegacyEventSystem();

        _gameOverPanel = CreateUiObject("GameOverPanel", canvasObject.transform);
        RectTransform panelRect = _gameOverPanel.AddComponent<RectTransform>();
        SetCenteredRect(panelRect, new Vector2(520f, 310f));
        Image panelImage = _gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0.03f, 0.05f, 0.1f, 0.94f);

        CreateText("GameOverTitle", _gameOverPanel.transform, "GAME OVER", 52, new Vector2(0f, 82f), new Vector2(460f, 70f));
        CreateButton("RestartButton", _gameOverPanel.transform, "Restart", new Vector2(-125f, -70f), RestartGame);
        CreateButton("QuitButton", _gameOverPanel.transform, "Quit", new Vector2(125f, -70f), QuitGame);

        _gameOverPanel.SetActive(false);
    }

    private static void EnsureLegacyEventSystem()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
        }

        foreach (BaseInputModule inputModule in eventSystem.GetComponents<BaseInputModule>())
        {
            inputModule.enabled = inputModule is StandaloneInputModule;
        }

        if (eventSystem.GetComponent<StandaloneInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
        }
    }

    private static void CreateButton(string name, Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = CreateUiObject(name, parent);
        RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
        SetCenteredRect(buttonRect, new Vector2(190f, 64f));
        buttonRect.anchoredPosition = position;

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.12f, 0.5f, 0.88f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(onClick);

        CreateText("Label", buttonObject.transform, label, 28, Vector2.zero, buttonRect.sizeDelta);
    }

    private static void CreateText(string name, Transform parent, string value, int fontSize, Vector2 position, Vector2 size)
    {
        GameObject textObject = CreateUiObject(name, parent);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        SetCenteredRect(textRect, size);
        textRect.anchoredPosition = position;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = value;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void SetCenteredRect(RectTransform rectTransform, Vector2 size)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
    }
}
