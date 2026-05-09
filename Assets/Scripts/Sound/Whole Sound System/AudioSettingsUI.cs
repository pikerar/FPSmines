using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI с тремя ползунками громкости.
/// Работает на любой сцене — главное меню, игра, экран поражения.
///
/// КАК НАСТРОИТЬ:
/// 1. Создай три UI Slider'а на Canvas.
/// 2. Повесь этот скрипт на любой GameObject.
/// 3. Перетащи слайдеры в поля musicSlider, environmentSlider, voiceSlider.
/// 4. (Опционально) перетащи три Text/TMP поля для отображения процентов.
/// </summary>
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

    // ──────────────────────────────────────────────
    // Lifecycle
    // ──────────────────────────────────────────────

    private void Start()
    {
        InitSliders();
    }

    // AudioManager может появиться позже (DontDestroyOnLoad), подождём
    private void Update()
    {
        if (!_initialized && AudioManager.Instance != null)
            InitSliders();
    }

    // ──────────────────────────────────────────────
    // Public (вызывай из кнопок: открыть/закрыть)
    // ──────────────────────────────────────────────

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

    // ──────────────────────────────────────────────
    // Slider callbacks (назначь через AddListener или Inspector → OnValueChanged)
    // ──────────────────────────────────────────────

    public void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.MusicVolume = value;
        UpdateLabel(musicLabel, value);
    }

    public void OnEnvironmentSliderChanged(float value)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.EnvironmentVolume = value;
        UpdateLabel(environmentLabel, value);
    }

    public void OnVoiceSliderChanged(float value)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.VoiceVolume = value;
        UpdateLabel(voiceLabel, value);
    }

    // ──────────────────────────────────────────────
    // Internals
    // ──────────────────────────────────────────────

    private void InitSliders()
    {
        if (AudioManager.Instance == null) return;
        _initialized = true;

        SetupSlider(musicSlider,       AudioManager.Instance.MusicVolume,       OnMusicSliderChanged);
        SetupSlider(environmentSlider, AudioManager.Instance.EnvironmentVolume,  OnEnvironmentSliderChanged);
        SetupSlider(voiceSlider,       AudioManager.Instance.VoiceVolume,        OnVoiceSliderChanged);

        UpdateLabel(musicLabel,       AudioManager.Instance.MusicVolume);
        UpdateLabel(environmentLabel, AudioManager.Instance.EnvironmentVolume);
        UpdateLabel(voiceLabel,       AudioManager.Instance.VoiceVolume);
    }

    private static void SetupSlider(Slider slider, float value, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.onValueChanged.RemoveAllListeners();
        slider.value = value; // установка value без вызова callback
        slider.onValueChanged.AddListener(callback);
    }
     private static void UpdateLabel(TMPro.TextMeshProUGUI label, float value)
    {
        if (label == null) return;
        label.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}
