using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Универсальная кнопка взаимодействия.
/// При нажатии: играет звук клика → выполняет список действий с задержкой каждого.
///
/// КАК НАСТРОИТЬ:
/// 1. Повесь на объект кнопки/интеркома.
/// 2. В поле clickClip — звук нажатия кнопки (идёт в Environment).
/// 3. В список actions добавляй действия:
///    — delay      = задержка перед этим действием (в секундах от предыдущего)
///    — label      = описание для читаемости в Inspector
///    — action     = любой метод: NpcIntercomVoice.Play(), RotatingMover.Activate() и т.д.
///
/// Пример для интеркома:
///    [0] delay=0   → NpcIntercomVoice.Play()     (сразу при нажатии)
///    [1] delay=3   → RotatingMover.Activate()    (через 3 сек после предыдущего)
///
/// 4. canRepeat — если false, кнопка срабатывает только один раз.
/// </summary>
public class InteractableButton : MonoBehaviour
{
    [System.Serializable]
    public class DelayedAction
    {
        [Tooltip("Задержка в секундах перед этим действием (отсчёт от предыдущего)")]
        public float delay = 0f;

        [Tooltip("Для читаемости — что делает это действие")]
        public string label = "";

        public UnityEvent action;
    }

    [Header("Взаимодействие")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private bool canRepeat = false;

    [Header("Звук кнопки (Environment)")]
    [SerializeField] private AudioClip clickClip;
    [Range(0f, 1f)]
    [SerializeField] private float clickVolume = 1f;
    [Range(0.5f, 2f)]
    [SerializeField] private float clickPitch = 1f;

    [Header("Действия при нажатии (выполняются по очереди с задержкой)")]
    [SerializeField] private DelayedAction[] actions;

    // ──────────────────────────────────────────────
    // State
    // ──────────────────────────────────────────────

    public bool IsActivated { get; private set; } = false;

    private Transform _player;

    // ──────────────────────────────────────────────
    // Lifecycle
    // ──────────────────────────────────────────────

    private void Start()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null) _player = playerGo.transform;
    }

    private void Update()
    {
        if (!canRepeat && IsActivated) return;
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist > interactDistance) return;

        if (InputHandler.Instance != null && InputHandler.Instance.InteractPressed)
            Activate();
    }

    // ──────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────

    public void Activate()
    {
        if (!canRepeat && IsActivated) return;

        IsActivated = true;

        // Звук клика — сразу
        if (clickClip != null)
            SoundPlayer.Instance?.PlayEnvironment(clickClip, transform.position, clickVolume, clickPitch);

        // Действия по очереди с задержками
        if (actions != null && actions.Length > 0)
            StartCoroutine(RunActions());
    }

    /// <summary>Сбросить кнопку (если canRepeat = false но нужен ручной сброс).</summary>
    public void Reset() => IsActivated = false;

    // ──────────────────────────────────────────────
    // Internals
    // ──────────────────────────────────────────────

    private IEnumerator RunActions()
    {
        foreach (var entry in actions)
        {
            if (entry.delay > 0f)
                yield return new WaitForSeconds(entry.delay);

            entry.action?.Invoke();
        }
    }
}