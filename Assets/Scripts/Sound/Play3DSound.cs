using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound3DPlayer : MonoBehaviour
{
    [Header("Настройки звука")]
    [Tooltip("Аудиоклип для проигрывания")]
    public AudioClip soundClip;

    [Tooltip("Проигрывать ли звук при старте")]
    public bool playOnStart = true;

    [Tooltip("Проигрывать ли звук в цикле")]
    public bool loop = false;

    [Tooltip("Громкость (0-1)")]
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource source;

    void Start()
    {
        source = GetComponent < AudioSource > ();

        SetupAudioSource();

        if (playOnStart && soundClip != null)
        {
            Play();
        }
    }

    void SetupAudioSource()
    {
        source.spatialBlend = 1f;
        source.spatialize = true;

        source.minDistance = 1f;
        source.maxDistance = 50f;
        source.rolloffMode = AudioRolloffMode.Linear;

        source.dopplerLevel = 1f;
        source.spread = 0f;

        source.volume = volume;
        source.loop = loop;
        source.playOnAwake = false;
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