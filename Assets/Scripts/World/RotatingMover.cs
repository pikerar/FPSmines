using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Универсальное движение объекта через поворот — ворота, шлагбаум, дверь.
///
/// КАК НАСТРОИТЬ:
/// 1. Повесь на объект ворот/шлагбаума.
/// 2. Укажи openRotation — угол назначения (например Vector3(0, 90, 0) для поворота на 90°).
/// 3. Из InteractableButton.OnInteract вызови Activate().
///
/// Опционально:
/// — canClose = true → повторный вызов Activate() закрывает обратно.
/// — delay     → задержка перед началом движения (пока играет голос NPC).
/// — OnOpened / OnClosed → события по завершению движения.
/// </summary>
public class RotatingMover : MonoBehaviour
{
    [Header("Поворот")]
    [Tooltip("Локальный угол в открытом состоянии")]
    [SerializeField] private Vector3 openRotation   = new Vector3(0f, 90f, 0f);
    [Tooltip("Локальный угол в закрытом состоянии (обычно Vector3.zero)")]
    [SerializeField] private Vector3 closedRotation = Vector3.zero;
    [SerializeField] private float   duration        = 1.5f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Задержка перед движением (сек)")]
    [SerializeField] private float delayBeforeMove = 0f;

    [Header("Поведение")]
    [Tooltip("Можно ли закрыть повторным вызовом Activate()")]
    [SerializeField] private bool canClose = false;

    [Header("События")]
    [SerializeField] private UnityEvent OnOpened;
    [SerializeField] private UnityEvent OnClosed;

    // ──────────────────────────────────────────────
    // State
    // ──────────────────────────────────────────────

    private bool _isOpen    = false;
    private bool _isMoving  = false;

    // ──────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────

    /// <summary>
    /// Открыть (или закрыть если canClose = true и уже открыто).
    /// Вешай в InteractableButton.OnInteract.
    /// </summary>
    public void Activate()
    {
        if (_isMoving) return;

        if (_isOpen && canClose)
            StartCoroutine(MoveRoutine(openRotation, closedRotation, onDone: () =>
            {
                _isOpen = false;
                OnClosed?.Invoke();
            }));
        else if (!_isOpen)
            StartCoroutine(MoveRoutine(closedRotation, openRotation, onDone: () =>
            {
                _isOpen = true;
                OnOpened?.Invoke();
            }));
    }

    /// <summary>Открыть принудительно.</summary>
    public void Open()
    {
        if (_isMoving || _isOpen) return;
        StartCoroutine(MoveRoutine(closedRotation, openRotation, onDone: () =>
        {
            _isOpen = true;
            OnOpened?.Invoke();
        }));
    }

    /// <summary>Закрыть принудительно.</summary>
    public void Close()
    {
        if (_isMoving || !_isOpen) return;
        StartCoroutine(MoveRoutine(openRotation, closedRotation, onDone: () =>
        {
            _isOpen = false;
            OnClosed?.Invoke();
        }));
    }

    // ──────────────────────────────────────────────
    // Internals
    // ──────────────────────────────────────────────

    private IEnumerator MoveRoutine(Vector3 from, Vector3 to, System.Action onDone)
    {
        _isMoving = true;

        if (delayBeforeMove > 0f)
            yield return new WaitForSeconds(delayBeforeMove);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
            transform.localEulerAngles = Vector3.LerpUnclamped(from, to, t);
            yield return null;
        }

        transform.localEulerAngles = to;
        _isMoving = false;
        onDone?.Invoke();
    }
}
