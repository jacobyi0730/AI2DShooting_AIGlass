using System;
using UnityEngine;
using UnityEngine.UI;

public class JC_Health : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHp = 3;
    [SerializeField, Min(0)] private int currentHp = 3;
    [SerializeField] private bool deactivateOnDeath = true;
    [SerializeField] private Vector3 healthBarLocalOffset = new Vector3(0f, 1.1f, 0f);
    [SerializeField] private Vector2 healthBarSize = new Vector2(120f, 18f);
    [SerializeField] private Color healthBarBackgroundColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] private Color healthBarFillColor = new Color(0.29f, 0.84f, 0.39f, 1f);

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsDead => currentHp <= 0;

    public event Action<int, int> HealthChanged;

    private Slider _healthSlider;

    private void Awake()
    {
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        if (currentHp == 0)
        {
            currentHp = maxHp;
        }

        EnsureHealthBar();
        NotifyHealthChanged();
    }

    public bool TakeDamage(int damageAmount)
    {
        if (IsDead || damageAmount <= 0)
        {
            return false;
        }

        currentHp = Mathf.Max(0, currentHp - damageAmount);
        NotifyHealthChanged();
        Debug.Log($"[{nameof(JC_Health)}] {gameObject.name} HP: {currentHp}/{maxHp}", this);

        if (currentHp == 0)
        {
            HandleDeath();
        }

        return true;
    }

    private void HandleDeath()
    {
        Debug.Log($"[{nameof(JC_Health)}] {gameObject.name} died.", this);

        if (deactivateOnDeath)
        {
            gameObject.SetActive(false);
        }
    }

    private void EnsureHealthBar()
    {
        if (_healthSlider != null)
        {
            return;
        }

        GameObject canvasObject = new GameObject("HealthBarCanvas");
        canvasObject.transform.SetParent(transform, false);
        canvasObject.transform.localPosition = healthBarLocalOffset;
        canvasObject.transform.localRotation = Quaternion.identity;
        canvasObject.transform.localScale = Vector3.one * 0.01f;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 100;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = healthBarSize;

        GameObject sliderObject = new GameObject("HealthBarSlider");
        sliderObject.transform.SetParent(canvasObject.transform, false);

        RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        Image backgroundImage = sliderObject.AddComponent<Image>();
        backgroundImage.color = healthBarBackgroundColor;

        _healthSlider = sliderObject.AddComponent<Slider>();
        _healthSlider.transition = Selectable.Transition.None;
        _healthSlider.interactable = false;
        _healthSlider.direction = Slider.Direction.LeftToRight;
        _healthSlider.minValue = 0f;
        _healthSlider.maxValue = maxHp;

        GameObject fillAreaObject = new GameObject("Fill Area");
        fillAreaObject.transform.SetParent(sliderObject.transform, false);

        RectTransform fillAreaRect = fillAreaObject.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(2f, 2f);
        fillAreaRect.offsetMax = new Vector2(-2f, -2f);

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(fillAreaObject.transform, false);

        RectTransform fillRect = fillObject.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Image fillImage = fillObject.AddComponent<Image>();
        fillImage.color = healthBarFillColor;

        _healthSlider.fillRect = fillRect;
        _healthSlider.targetGraphic = backgroundImage;
    }

    private void NotifyHealthChanged()
    {
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = maxHp;
            _healthSlider.value = currentHp;
        }

        HealthChanged?.Invoke(currentHp, maxHp);
    }
}
