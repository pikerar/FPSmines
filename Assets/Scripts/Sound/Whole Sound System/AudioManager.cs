using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Центральный менеджер звука. Синглтон, переживает смену сцен.
///
/// КАК НАСТРОИТЬ В РЕДАКТОРЕ:
/// 1. Создай GameObject "AudioManager" на сцене MainMenu.
/// 2. Повесь этот скрипт.
/// 3. В Project создай Audio Mixer: Assets → Create → Audio Mixer → назови "GameAudioMixer".
/// 4. В Mixer добавь три дочерние группы: Music, Environment, Voice.
/// 5. Для каждой группы: выдели → в Inspector нажми на параметр Volume → правой кнопкой → Expose.
///    Переименуй exposed параметры в: "MusicVolume", "EnvironmentVolume", "VoiceVolume".
/// 6. Перетащи Mixer и три группы в поля этого компонента в Inspector.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup environmentGroup;
    [SerializeField] private AudioMixerGroup voiceGroup;

    // Exposed parameter names в AudioMixer
    private const string MusicParam       = "MusicVolume";
    private const string EnvironmentParam = "EnvironmentVolume";
    private const string VoiceParam       = "VoiceVolume";

    public SoundSettings Settings { get; private set; }

    // ──────────────────────────────────────────────
    // Public accessors для групп (нужны SoundPlayer и AmbientController)
    // ──────────────────────────────────────────────
    public AudioMixerGroup MusicGroup       => musicGroup;
    public AudioMixerGroup EnvironmentGroup => environmentGroup;
    public AudioMixerGroup VoiceGroup       => voiceGroup;

    // ──────────────────────────────────────────────
    // Lifecycle
    // ──────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Settings = SoundSettings.Load();
        ApplyAll();
    }

    private void OnApplicationQuit() => Settings.Save();
    private void OnApplicationPause(bool pause) { if (pause) Settings.Save(); }

    // ──────────────────────────────────────────────
    // Volume API (0..1 → dB)
    // ──────────────────────────────────────────────

    public float MusicVolume
    {
        get => Settings.musicVolume;
        set
        {
            Settings.musicVolume = Mathf.Clamp01(value);
            SetMixerVolume(MusicParam, Settings.musicVolume);
        }
    }

    public float EnvironmentVolume
    {
        get => Settings.environmentVolume;
        set
        {
            Settings.environmentVolume = Mathf.Clamp01(value);
            SetMixerVolume(EnvironmentParam, Settings.environmentVolume);
        }
    }

    public float VoiceVolume
    {
        get => Settings.voiceVolume;
        set
        {
            Settings.voiceVolume = Mathf.Clamp01(value);
            SetMixerVolume(VoiceParam, Settings.voiceVolume);
        }
    }

    /// <summary>Сохраняет текущие настройки в JSON немедленно.</summary>
    public void SaveSettings() => Settings.Save();

    // ──────────────────────────────────────────────
    // Internals
    // ──────────────────────────────────────────────

    private void ApplyAll()
    {
        SetMixerVolume(MusicParam,       Settings.musicVolume);
        SetMixerVolume(EnvironmentParam, Settings.environmentVolume);
        SetMixerVolume(VoiceParam,       Settings.voiceVolume);
    }

    /// <summary>
    /// Конвертирует линейное значение (0..1) в децибелы и применяет к миксеру.
    /// При value == 0 ставим -80 dB (тишина), иначе 20*log10(value).
    /// </summary>
    private void SetMixerVolume(string paramName, float linearValue)
    {
        if (masterMixer == null) return;
        float db = linearValue > 0.0001f ? Mathf.Log10(linearValue) * 20f : -80f;
        masterMixer.SetFloat(paramName, db);
    }
}
