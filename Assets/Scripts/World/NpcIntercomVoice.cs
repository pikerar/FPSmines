using System.Collections;
using UnityEngine;

/// <summary>
/// Голос NPC через интерком — 3D пространственный звук из точки объекта.
/// Идёт в Voice группу миксера.
///
/// КАК НАСТРОИТЬ:
/// 1. Повесь на объект интеркома (откуда "говорит" NPC).
/// 2. Добавь AudioClip'ы реплик.
/// 3. Из InteractableButton.OnInteract вызови:
///    — Play(0)       — конкретную реплику по индексу
///    — PlayDelayed() — с задержкой (например, пока играет звук кнопки)
///
/// Настройки 3D звука (дистанция, rolloff) — прямо в Inspector этого компонента.
/// </summary>
public class NpcIntercomVoice : MonoBehaviour
{
    [Header("Реплики")]
    [SerializeField] private AudioClip[] voiceLines;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    [Header("3D настройки")]
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 15f;

    [Header("Задержка перед репликой (сек)")]
    [SerializeField] private float delayBeforeVoice = 0f;

    // ──────────────────────────────────────────────
    // Внутренний AudioSource для 3D (не идёт в пул —
    // нужны spatialBlend + rolloff настройки на объекте)
    // ──────────────────────────────────────────────

    private AudioSource _source;

    private void Awake()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _source.spatialBlend  = 1f;
        _source.rolloffMode   = AudioRolloffMode.Linear;
        _source.minDistance   = minDistance;
        _source.maxDistance   = maxDistance;
        _source.playOnAwake   = false;

        // Подключаем к Voice группе миксера
        // Start гарантированно после AudioManager.Awake
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
            _source.outputAudioMixerGroup = AudioManager.Instance.VoiceGroup;
    }

    // ──────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────

    /// <summary>Воспроизвести реплику по индексу.</summary>
    public void Play(int index)
    {
        if (voiceLines == null || index < 0 || index >= voiceLines.Length) return;
        StartCoroutine(PlayRoutine(voiceLines[index]));
    }

    /// <summary>Воспроизвести первую реплику (удобно вешать в OnInteract без параметра).</summary>
    public void Play() => Play(0);

    /// <summary>Воспроизвести случайную реплику.</summary>
    public void PlayRandom()
    {
        if (voiceLines == null || voiceLines.Length == 0) return;
        Play(Random.Range(0, voiceLines.Length));
    }

    /// <summary>Остановить текущую реплику.</summary>
    public void Stop() => _source.Stop();

    // ──────────────────────────────────────────────
    // Internals
    // ──────────────────────────────────────────────

    private IEnumerator PlayRoutine(AudioClip clip)
    {
        if (clip == null) yield break;

        if (delayBeforeVoice > 0f)
            yield return new WaitForSeconds(delayBeforeVoice);

        _source.volume = volume;
        _source.PlayOneShot(clip);
    }
}
