using UnityEngine;
using UnityEngine.UI;


public class AudioSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider environmentSlider;
    [SerializeField] private Slider voiceSlider;

    [Header("Labels (опционально — можно оставить пустыми)")]
    [SerializeField] private TMPro.TextMeshProUGUI musicLabel;
    [SerializeField] private TMPro.TextMeshProUGUI environmentLabel;
    [SerializeField] private TMPro.TextMeshProUGUI voiceLabel;

    [Header("Панель настроек (если нужно скрывать/показывать)")]
    [SerializeField] private GameObject settingsPanel;

    private bool _initialized;
    private bool _isInitializing;

   
    private void Start()
    {
        InitSliders();
    }

    private void Update()
    {
        if (!_initialized && AudioManager.Instance != null)
            InitSliders();
    }

    public void ShowSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void HideSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        AudioManager.Instance?.SaveSettings();
    }

    public void ToggleSettings()
    {
        if (settingsPanel == null) return;
        bool next = !settingsPanel.activeSelf;
        settingsPanel.SetActive(next);
        if (!next) AudioManager.Instance?.SaveSettings();
    }

    public void OnMusicSliderChanged(float value)
    {
        if (_isInitializing || AudioManager.Instance == null) return; 
        AudioManager.Instance.MusicVolume = value;
        UpdateLabel(musicLabel, value);
    }

    public void OnEnvironmentSliderChanged(float value)
    {
        if (_isInitializing || AudioManager.Instance == null) return; 
        AudioManager.Instance.EnvironmentVolume = value;
        UpdateLabel(environmentLabel, value);
    }

    public void OnVoiceSliderChanged(float value)
    {
        if (_isInitializing || AudioManager.Instance == null) return; 
        AudioManager.Instance.VoiceVolume = value;
        UpdateLabel(voiceLabel, value);
    }

    private void InitSliders()
    {
        if (AudioManager.Instance == null) return;
        _initialized = true;
        _isInitializing = true; 

        SetupSlider(musicSlider, AudioManager.Instance.MusicVolume, OnMusicSliderChanged);
        SetupSlider(environmentSlider, AudioManager.Instance.EnvironmentVolume, OnEnvironmentSliderChanged);
        SetupSlider(voiceSlider, AudioManager.Instance.VoiceVolume, OnVoiceSliderChanged);

        UpdateLabel(musicLabel, AudioManager.Instance.MusicVolume);
        UpdateLabel(environmentLabel, AudioManager.Instance.EnvironmentVolume);
        UpdateLabel(voiceLabel, AudioManager.Instance.VoiceVolume);

        _isInitializing = false; 

        AudioManager.Instance.ApplyAll();
    }

    private static void SetupSlider(Slider slider, float value, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.onValueChanged.RemoveAllListeners();
        slider.value = value;
        slider.onValueChanged.AddListener(callback);
    }

    private static void UpdateLabel(TMPro.TextMeshProUGUI label, float value)
    {
        if (label == null) return;
        label.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}
