using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    public bool IsActivated { get; private set; } = false;

    private Transform _player;

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


    public void Activate()
    {
        if (!canRepeat && IsActivated) return;

        IsActivated = true;

        if (clickClip != null)
            SoundPlayer.Instance?.PlayEnvironment(clickClip, transform.position, clickVolume, clickPitch);

        if (actions != null && actions.Length > 0)
            StartCoroutine(RunActions());
    }

    public void Reset() => IsActivated = false;

    private IEnumerator RunActions()
    {
        foreach (var entry in actions)
        {
            if (entry.delay > 0f)
                yield return new WaitForSeconds(entry.delay);

            entry.action?.Invoke();
        }
    }

    public void ForceActivated()
    {
        IsActivated = true;
    }

}