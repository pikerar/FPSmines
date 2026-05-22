using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup environmentGroup;
    [SerializeField] private AudioMixerGroup voiceGroup;

    private const string MusicParam       = "MusicVolume";
    private const string EnvironmentParam = "EnvironmentVolume";
    private const string VoiceParam       = "VoiceVolume";

    public SoundSettings Settings { get; private set; }

    public AudioMixerGroup MusicGroup       => musicGroup;
    public AudioMixerGroup EnvironmentGroup => environmentGroup;
    public AudioMixerGroup VoiceGroup       => voiceGroup;

    private void Awake()
    {
        Debug.Log($"[AudioManager] Awake. Instance exists: {Instance != null}, this={gameObject.name}");

        if (Instance != null && Instance != this)
        {
            Debug.Log("[AudioManager] Duplicate destroyed");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Settings = SoundSettings.Load();
        Debug.Log($"[AudioManager] Loaded settings: music={Settings.musicVolume}");
        ApplyAll();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnApplicationQuit() => Settings.Save();
    private void OnApplicationPause(bool pause) { if (pause) Settings.Save(); }

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

    public void SaveSettings() => Settings.Save();


    public void ApplyAll()
    {
        Debug.Log($"[AudioManager] ApplyAll: music={Settings.musicVolume}, env={Settings.environmentVolume}, voice={Settings.voiceVolume}");
        SetMixerVolume(MusicParam,       Settings.musicVolume);
        SetMixerVolume(EnvironmentParam, Settings.environmentVolume);
        SetMixerVolume(VoiceParam,       Settings.voiceVolume);
    }

    private void SetMixerVolume(string paramName, float linearValue)
    {
        if (masterMixer == null) { Debug.LogWarning("[AudioManager] masterMixer is NULL!"); return; }
        float db = linearValue > 0.0001f ? Mathf.Log10(linearValue) * 20f : -80f;
        bool ok = masterMixer.SetFloat(paramName, db);
        Debug.Log($"[AudioManager] SetMixerVolume: {paramName} = {linearValue} → {db}dB, success={ok}");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAll();
    }
}
