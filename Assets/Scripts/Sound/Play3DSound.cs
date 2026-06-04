using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Sound3DPlayer : MonoBehaviour
{
    [Header("Клип")]
    public AudioClip soundClip;
    public bool playOnStart = true;

    [Header("Микшер")]
    [Tooltip("Группа микшера (Ambience, Music, Voice, SFX)")]
    public AudioMixerGroup mixerGroup; 

    [Header("3D настройки")]
    public float minDistance = 1f;
    public float maxDistance = 50f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

    [Header("Остальное")]
    public bool loop = false;
    [Range(0f, 1f)] public float volume = 1f;

    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        SetupAudioSource();

        if (playOnStart && soundClip != null)
            Play();
    }

    void SetupAudioSource()
    {
        source.spatialBlend = 1f;
        source.spatialize = true;

        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = rolloffMode;

        source.dopplerLevel = 1f;
        source.spread = 0f;

        source.volume = volume;
        source.loop = loop;
        source.playOnAwake = false;

        if (mixerGroup != null)
            source.outputAudioMixerGroup = mixerGroup;
    }

    public void Play()
    {
        if (soundClip == null)
        {
            Debug.LogWarning("AudioClip не назначен!", this);
            return;
        }

        source.clip = soundClip;
        source.Play();
    }

    public void Play(AudioClip clip)
    {
        soundClip = clip;
        Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void Pause()
    {
        source.Pause();
    }

    public bool IsPlaying()
    {
        return source.isPlaying;
    }
}