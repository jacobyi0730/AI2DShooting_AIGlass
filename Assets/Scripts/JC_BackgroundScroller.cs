using UnityEngine;

/// <summary>
/// Scrolls the assigned background material vertically by updating its UV offset.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class JC_BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.08f;
    [SerializeField] private Renderer targetRenderer;

    private static readonly int BaseMapProperty = Shader.PropertyToID("_BaseMap");

    private Material _material;
    private Vector2 _offset;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (targetRenderer == null)
        {
            enabled = false;
            return;
        }

        _material = targetRenderer.material;
        _offset = _material.HasProperty(BaseMapProperty)
            ? _material.GetTextureOffset(BaseMapProperty)
            : _material.mainTextureOffset;
    }

    private void Update()
    {
        _offset.y = Mathf.Repeat(_offset.y - (scrollSpeed * Time.deltaTime), 1f);

        if (_material.HasProperty(BaseMapProperty))
        {
            _material.SetTextureOffset(BaseMapProperty, _offset);
        }
        else
        {
            _material.mainTextureOffset = _offset;
        }
    }
}
