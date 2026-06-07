using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Активирует WorldHint по одному из нескольких условий:
/// - OnTriggerEnter (игрок заходит в зону)
/// - Interaction (кнопка / объект взаимодействия — вызвать Activate() вручную)
/// - UnityEvent (любое другое событие через инспектор)
/// - Автозапуск через N секунд после старта
/// </summary>
public class HintTrigger : MonoBehaviour
{
    [Header("Подсказка")]
    public WorldHintCompass targetHint;
    [TextArea(2, 5)]
    public string hintMessage = "Подойди к двери!";

    [Header("Тип триггера")]
    public TriggerType triggerType = TriggerType.Interaction;

    [Header("Триггер-зона (TriggerType.Zone)")]
    [Tooltip("Тег объекта, который активирует триггер (обычно 'Player')")]
    public string playerTag = "Player";

    [Header("Автозапуск (TriggerType.AutoStart)")]
    public float autoStartDelay = 0f;

    [Header("Настройки")]
    [Tooltip("Активировать только один раз")]
    public bool activateOnce = true;

    [Header("Дополнительные события")]
    public UnityEvent onHintShown;

    public enum TriggerType
    {
        /// <summary>Вызвать Activate() из кода/другого скрипта/кнопки UI</summary>
        Interaction,
        /// <summary>Коллайдер-триггер на этом объекте</summary>
        Zone,
        /// <summary>Запускается через delay секунд после Awake</summary>
        AutoStart,
    }

    private bool _activated;

    private void Start()
    {
        if (triggerType == TriggerType.AutoStart)
            Invoke(nameof(Activate), autoStartDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerType != TriggerType.Zone) return;
        if (!other.CompareTag(playerTag)) return;
        Activate();
    }

    /// <summary>
    /// Вызвать из кнопки (UI Button OnClick), другого скрипта, анимации и т.д.
    /// </summary>
    public void Activate()
    {
        if (activateOnce && _activated) return;
        if (targetHint == null)
        {
            Debug.LogWarning($"[HintTrigger] {name}: targetHint не назначен!", this);
            return;
        }

        _activated = true;
        targetHint.Show(hintMessage);
        onHintShown?.Invoke();
    }

    /// <summary>Сбросить флаг — позволить активировать снова</summary>
    public void Reset() => _activated = false;
}