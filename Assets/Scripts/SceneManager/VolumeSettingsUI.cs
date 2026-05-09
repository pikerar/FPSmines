using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider environmentSlider;
    [SerializeField] private Slider voiceSlider;

    private AudioManager audioManager;

    private void OnEnable()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null) return;

        musicSlider.SetValueWithoutNotify(audioManager.MusicVolume);
        environmentSlider.SetValueWithoutNotify(audioManager.EnvironmentVolume);
        voiceSlider.SetValueWithoutNotify(audioManager.VoiceVolume);

        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        environmentSlider.onValueChanged.AddListener(OnEnvironmentChanged);
        voiceSlider.onValueChanged.AddListener(OnVoiceChanged);
    }

    private void OnDisable()
    {
        if (audioManager == null) return;

        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        environmentSlider.onValueChanged.RemoveListener(OnEnvironmentChanged);
        voiceSlider.onValueChanged.RemoveListener(OnVoiceChanged);

        audioManager.SaveSettings();
    }

    private void OnMusicChanged(float value)
    {
        audioManager.MusicVolume = value;
    }

    private void OnEnvironmentChanged(float value)
    {
        audioManager.EnvironmentVolume = value;
    }

    private void OnVoiceChanged(float value)
    {
        audioManager.VoiceVolume = value;
    }
}