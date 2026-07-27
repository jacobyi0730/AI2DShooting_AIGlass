using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JC_ScoreTextBinder : MonoBehaviour
{
    [SerializeField] private JC_ScoreManager scoreManager;
    [SerializeField] private string scorePrefix = "Score: ";
    [SerializeField] private Vector2 screenPadding = new Vector2(24f, 24f);
    [SerializeField] private int fontSize = 28;
    [SerializeField] private Color textColor = Color.white;

    private static TMP_FontAsset _runtimeFontAsset;

    private TextMeshProUGUI _scoreText;

    private void Awake()
    {
        if (scoreManager == null)
        {
            scoreManager = GetComponent<JC_ScoreManager>();
        }

        EnsureScoreText();
    }

    private void OnEnable()
    {
        ResolveScoreManager();
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged += HandleScoreChanged;
            HandleScoreChanged(scoreManager.CurrentScore);
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged -= HandleScoreChanged;
        }
    }

    private void EnsureScoreText()
    {
        if (_scoreText != null)
        {
            return;
        }

        GameObject canvasObject = new GameObject("ScoreCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(768f, 1024f);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject textObject = new GameObject("ScoreText");
        textObject.transform.SetParent(canvasObject.transform, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(-screenPadding.x, -screenPadding.y);
        rectTransform.sizeDelta = new Vector2(260f, 48f);

        _scoreText = textObject.AddComponent<TextMeshProUGUI>();
        _scoreText.font = GetRuntimeFontAsset();
        _scoreText.fontSize = fontSize;
        _scoreText.alignment = TextAlignmentOptions.MidlineRight;
        _scoreText.enableWordWrapping = false;
        _scoreText.overflowMode = TextOverflowModes.Overflow;
        _scoreText.color = textColor;
        _scoreText.text = $"{scorePrefix}0";
    }

    private void ResolveScoreManager()
    {
        if (scoreManager == null)
        {
            scoreManager = JC_ScoreManager.Instance;
        }
    }

    private void HandleScoreChanged(int score)
    {
        EnsureScoreText();
        _scoreText.text = $"{scorePrefix}{score}";
    }

    private static TMP_FontAsset GetRuntimeFontAsset()
    {
        if (_runtimeFontAsset != null)
        {
            return _runtimeFontAsset;
        }

        Font sourceFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _runtimeFontAsset = TMP_FontAsset.CreateFontAsset(sourceFont);
        return _runtimeFontAsset;
    }
}
