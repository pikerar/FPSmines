using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Иерархия объекта:
///
/// HintObject                    ← WorldHint.cs, SubtitleDisplay3D.cs
///   ├── Quad                    ← Billboard3D.cs, visualRoot
///   │     └── HintText          ← TextMeshPro 3D
///   └── HintDismissZone         ← Collider (Is Trigger), HintDismisser.cs
///
/// Отдельно на Canvas (Screen Space - Overlay):
///   └── HintIndicator           ← offscreenIndicator (RectTransform)
///         └── Arrow Image       ← arrowImage, крутится к цели
/// </summary>
[RequireComponent(typeof(SubtitleDisplay3D))]
public class WorldHint : MonoBehaviour
{
    [Header("Камера игрока")]
    [Tooltip("Перетащи камеру игрока вручную")]
    public Camera playerCamera;

    [Header("Визуальная часть (Quad + HintText)")]
    [Tooltip("Перетащи сюда дочерний Quad")]
    public Transform visualRoot;
    public Billboard3D billboard;
    public SubtitleDisplay3D subtitle;

    [Header("Автомасштаб по дистанции")]
    public bool autoScale = true;
    [Tooltip("Масштаб = 1 на этой дистанции от игрока")]
    public float referenceDistance = 5f;
    public float minScale = 0.3f;
    public float maxScale = 2.0f;

    [Header("UI индикатор (когда 3D объект вне экрана)")]
    [Tooltip("RectTransform UI элемента-стрелки на Canvas")]
    public RectTransform offscreenIndicator;
    [Tooltip("Image стрелки внутри индикатора — будет вращаться к цели")]
    public RectTransform arrowImage;
    [Tooltip("Отступ от края экрана в пикселях")]
    public float screenEdgePadding = 60f;

    // ── приватное ────────────────────────────────────────────────

    private Camera _cam;
    private Vector3 _worldAnchor;
    private Vector3 _baseVisualScale;
    private bool _active;

    // ── API ──────────────────────────────────────────────────────

    public void Show(string text)
    {
        Prepare();
        subtitle.ShowLine(text);
    }

    public void Show(string text, float holdDuration)
    {
        Prepare();
        subtitle.ShowLine(text, holdDuration);
    }

    public void ShowSequence(string[] lines, float[] durations)
    {
        Prepare();
        subtitle.ShowSequence(lines, durations);
    }

    public void Hide()
    {
        if (!_active) return;
        _active = false;
        SetIndicator(false);
        subtitle.Hide();
        Invoke(nameof(HideVisual), subtitle.fadeTime + 0.05f);
    }

    // ── Unity ────────────────────────────────────────────────────

    private void Awake()
    {
        if (subtitle == null) subtitle = GetComponent<SubtitleDisplay3D>();
        if (billboard == null && visualRoot != null)
            billboard = visualRoot.GetComponent<Billboard3D>();
        if (billboard == null)
            billboard = GetComponent<Billboard3D>();

        _cam = playerCamera;
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) Debug.LogWarning("[WorldHint] Камера не назначена!", this);

        _worldAnchor = transform.position;

        if (visualRoot != null)
            _baseVisualScale = visualRoot.localScale;

        _active = false;
    }

    private void Start()
    {
        HideVisual();
        SetIndicator(false);
        if (subtitle != null) subtitle.Hide();
    }

    private void LateUpdate()
    {
        if (_cam == null)
        {
            _cam = playerCamera != null ? playerCamera : Camera.main;
            if (_cam == null) return;
        }

        if (!_active) return;
        if (visualRoot == null) return;

        HandleScale();
        HandleVisibility();
    }

    // ── Подготовка ───────────────────────────────────────────────

    private void Prepare()
    {
        _active = true;
        if (visualRoot != null)
        {
            visualRoot.gameObject.SetActive(true);
            visualRoot.position = _worldAnchor;
            visualRoot.localScale = _baseVisualScale;
        }
        SetIndicator(false);
    }

    private void HideVisual()
    {
        if (visualRoot != null)
            visualRoot.gameObject.SetActive(false);
    }

    // ── Масштаб 3D объекта ───────────────────────────────────────

    private void HandleScale()
    {
        if (!autoScale) return;
        float dist = Vector3.Distance(_cam.transform.position, _worldAnchor);
        float s = Mathf.Clamp(dist / Mathf.Max(referenceDistance, 0.001f), minScale, maxScale);
        visualRoot.localScale = _baseVisualScale * s;
    }

    // ── Видимость: 3D или UI ─────────────────────────────────────

    private void HandleVisibility()
    {
        // Переводим мировой якорь в viewport
        Vector3 vp = _cam.WorldToViewportPoint(_worldAnchor);

        bool isBehind = vp.z < 0f;
        bool isOnScreen = !isBehind
                          && vp.x >= 0f && vp.x <= 1f
                          && vp.y >= 0f && vp.y <= 1f;

        if (isOnScreen)
        {
            // 3D объект виден — UI прячем, Quad на месте
            visualRoot.position = _worldAnchor;
            SetIndicator(false);
        }
        else
        {
            // 3D объект вне экрана — Quad прячем, UI показываем
            // Quad ставим на якорь (не двигаем перед камерой)
            visualRoot.position = _worldAnchor;
            SetIndicator(true);
            UpdateIndicatorPosition(vp, isBehind);
        }
    }

    // ── UI индикатор ─────────────────────────────────────────────

    private void SetIndicator(bool state)
    {
        if (offscreenIndicator != null && offscreenIndicator.gameObject.activeSelf != state)
            offscreenIndicator.gameObject.SetActive(state);
    }

    private void UpdateIndicatorPosition(Vector3 vp, bool isBehind)
    {
        if (offscreenIndicator == null) return;

        // Получаем размер экрана
        float screenW = Screen.width;
        float screenH = Screen.height;
        float pad = screenEdgePadding;

        // Центр экрана в пикселях
        Vector2 screenCenter = new Vector2(screenW * 0.5f, screenH * 0.5f);

        // Переводим viewport в пиксели
        Vector2 screenPos = new Vector2(vp.x * screenW, vp.y * screenH);

        // Если якорь строго за спиной — инвертируем позицию
        if (isBehind)
            screenPos = screenCenter * 2f - screenPos;

        // Направление от центра экрана к цели
        Vector2 dir = (screenPos - screenCenter).normalized;

        // Находим точку на краю экрана вдоль этого направления с отступом
        Vector2 clampedPos = ClampToScreenEdge(screenCenter, dir, screenW, screenH, pad);

        // Переводим в anchoredPosition (Canvas Screen Space Overlay)
        // RectTransform должен быть дочерним Canvas с pivot 0.5/0.5
        offscreenIndicator.position = new Vector3(clampedPos.x, clampedPos.y, 0f);

        // Вращаем стрелку так чтобы она указывала в сторону якоря
        if (arrowImage != null)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // -90 потому что стрелка обычно рисуется "вверх" (0° = вверх)
            arrowImage.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    /// <summary>
    /// Находит точку пересечения луча из центра экрана в направлении dir
    /// с прямоугольником экрана (с отступом pad пикселей от краёв).
    /// </summary>
    private Vector2 ClampToScreenEdge(Vector2 center, Vector2 dir,
                                       float screenW, float screenH, float pad)
    {
        float left = pad;
        float right = screenW - pad;
        float bottom = pad;
        float top = screenH - pad;

        // Проверяем пересечение с каждой из 4 границ и берём ближайшую
        float tMin = float.MaxValue;

        if (dir.x != 0f)
        {
            float t1 = (left - center.x) / dir.x;
            float t2 = (right - center.x) / dir.x;
            if (t1 > 0) tMin = Mathf.Min(tMin, t1);
            if (t2 > 0) tMin = Mathf.Min(tMin, t2);
        }
        if (dir.y != 0f)
        {
            float t3 = (bottom - center.y) / dir.y;
            float t4 = (top - center.y) / dir.y;
            if (t3 > 0) tMin = Mathf.Min(tMin, t3);
            if (t4 > 0) tMin = Mathf.Min(tMin, t4);
        }

        if (tMin == float.MaxValue) return center;

        Vector2 result = center + dir * tMin;
        result.x = Mathf.Clamp(result.x, left, right);
        result.y = Mathf.Clamp(result.y, bottom, top);
        return result;
    }

    private void OnValidate() => _worldAnchor = transform.position;
}