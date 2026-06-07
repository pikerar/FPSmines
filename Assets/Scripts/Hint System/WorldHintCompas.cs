using UnityEngine;

/// <summary>
/// Альтернативный режим индикатора:
/// стрелка фиксирована в центре экрана (или со смещением)
/// и вращается указывая направление к 3D подсказке.
/// 
/// Используется ВМЕСТО WorldHint.cs — это отдельный вариант.
/// 
/// Иерархия та же:
/// HintObject                    ← этот скрипт + SubtitleDisplay3D
///   ├── Quad                    ← Billboard3D, visualRoot
///   │     └── HintText          ← TextMeshPro 3D
///   └── HintDismissZone         ← Collider + HintDismisser
///
/// Canvas (Screen Space Overlay):
///   └── CompassArrow            ← compassArrow (RectTransform)
///         └── ArrowImage        ← arrowImage, сама картинка стрелки
/// </summary>
[RequireComponent(typeof(SubtitleDisplay3D))]
public class WorldHintCompass : MonoBehaviour
{
    [Header("Камера игрока")]
    public Camera playerCamera;

    [Header("Визуальная часть (Quad + HintText)")]
    [Tooltip("Перетащи сюда дочерний Quad")]
    public Transform visualRoot;
    public Billboard3D billboard;
    public SubtitleDisplay3D subtitle;

    [Header("Автомасштаб по дистанции")]
    public bool autoScale = true;
    [Tooltip("Масштаб = 1 на этой дистанции")]
    public float referenceDistance = 5f;
    public float minScale = 0.3f;
    public float maxScale = 2.0f;

    [Header("Компас-стрелка (UI)")]
    [Tooltip("RectTransform контейнера стрелки — его позиция = центр вращения")]
    public RectTransform compassArrow;
    [Tooltip("Дочерний RectTransform самой картинки стрелки (крутится)")]
    public RectTransform arrowImage;
    [Tooltip("Смещение от центра экрана в пикселях (например 0,-80 = чуть ниже)")]
    public Vector2 compassOffset = new Vector2(0f, -80f);
    [Tooltip("Плавность вращения стрелки (0 = мгновенно, 15 = плавно)")]
    [Range(0f, 30f)]
    public float rotationSmoothing = 12f;

    // ── приватное ────────────────────────────────────────────────

    private Camera _cam;
    private Vector3 _worldAnchor;
    private Vector3 _baseVisualScale;
    private bool _active;
    private float _currentAngle;

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
        SetCompass(false);
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
        if (_cam == null) Debug.LogWarning("[WorldHintCompass] Камера не назначена!", this);

        _worldAnchor = transform.position;
        if (visualRoot != null) _baseVisualScale = visualRoot.localScale;

        _active = false;
    }

    private void Start()
    {
        HideVisual();
        SetCompass(false);
        if (subtitle != null) subtitle.Hide();

        // Фиксируем позицию компаса на старте и при изменении offset
        PositionCompassAnchor();
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
        PositionCompassAnchor();
        SetCompass(false);
    }

    private void HideVisual()
    {
        if (visualRoot != null)
            visualRoot.gameObject.SetActive(false);
    }

    // ── Масштаб ──────────────────────────────────────────────────

    private void HandleScale()
    {
        if (!autoScale) return;
        float dist = Vector3.Distance(_cam.transform.position, _worldAnchor);
        float s = Mathf.Clamp(dist / Mathf.Max(referenceDistance, 0.001f), minScale, maxScale);
        visualRoot.localScale = _baseVisualScale * s;
    }

    // ── Видимость ────────────────────────────────────────────────

    private void HandleVisibility()
    {
        Vector3 vp = _cam.WorldToViewportPoint(_worldAnchor);

        bool isBehind = vp.z < 0f;
        bool isOnScreen = !isBehind
                          && vp.x >= 0f && vp.x <= 1f
                          && vp.y >= 0f && vp.y <= 1f;

        visualRoot.position = _worldAnchor;

        // Компас всегда виден пока подсказка активна, стрелка крутится к цели
        SetCompass(true);
        UpdateCompassRotation(vp, isBehind);
    }

    // ── Компас ───────────────────────────────────────────────────

    private void SetCompass(bool state)
    {
        if (compassArrow != null && compassArrow.gameObject.activeSelf != state)
            compassArrow.gameObject.SetActive(state);
    }

    /// <summary>
    /// Ставим контейнер компаса в центр экрана + offset.
    /// Делаем это через screen position чтобы работало с любым Canvas.
    /// </summary>
    private void PositionCompassAnchor()
    {
        if (compassArrow == null) return;
        Vector2 center = new Vector2(Screen.width * 0.5f + compassOffset.x,
                                     Screen.height * 0.5f + compassOffset.y);
        compassArrow.position = new Vector3(center.x, center.y, 0f);
    }

    private void UpdateCompassRotation(Vector3 vp, bool isBehind)
    {
        if (arrowImage == null) return;

        // Пиксельные координаты якоря на экране
        Vector2 screenCenter = new Vector2(
            Screen.width * 0.5f + compassOffset.x,
            Screen.height * 0.5f + compassOffset.y);

        Vector2 screenPos = new Vector2(vp.x * Screen.width, vp.y * Screen.height);

        // Если якорь строго за спиной — инвертируем относительно центра
        if (isBehind)
        {
            screenPos = screenCenter * 2f - screenPos;
        }

        // Направление от центра компаса к якорю
        Vector2 dir = (screenPos - screenCenter).normalized;

        // Угол: Atan2 даёт угол от оси X, стрелка обычно рисуется "вверх"
        // поэтому -90°
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        // Плавное вращение через угловую интерполяцию
        if (rotationSmoothing > 0f)
        {
            _currentAngle = Mathf.LerpAngle(_currentAngle, targetAngle,
                                             Time.deltaTime * rotationSmoothing);
        }
        else
        {
            _currentAngle = targetAngle;
        }

        arrowImage.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
    }

    private void OnValidate() => _worldAnchor = transform.position;
}