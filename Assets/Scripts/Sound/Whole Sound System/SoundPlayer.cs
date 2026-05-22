using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [Header("Pool")]
    [SerializeField] private int poolSize = 20;

    private readonly List<AudioSource> _pool = new();

    private readonly Dictionary<string, AudioSource> _voiceSources = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        for (int i = 0; i < poolSize; i++)
            _pool.Add(CreatePooledSource($"PooledSound_{i}"));
    }

    public void PlayEnvironment(AudioClip clip, Vector3 position, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        var src = GetFreeSource();
        if (src == null) return;

        ConfigureSource(src, clip, AudioManager.Instance?.EnvironmentGroup, pitch);
        src.transform.position = position;
        src.spatialBlend = 1f;   // 3D
        src.volume = volumeScale;
        src.Play();
        StartCoroutine(ReturnAfterPlay(src, clip.length / Mathf.Max(pitch, 0.01f)));
    }
    public void PlayVoice(AudioClip clip, string ownerId = null, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        if (ownerId != null) StopVoice(ownerId);

        var src = GetFreeSource();
        if (src == null) return;

        ConfigureSource(src, clip, AudioManager.Instance?.VoiceGroup, pitch);
        src.spatialBlend = 0f;   
        src.volume = volumeScale;
        src.Play();

        if (ownerId != null) _voiceSources[ownerId] = src;
        StartCoroutine(ReturnAfterPlay(src, clip.length / Mathf.Max(pitch, 0.01f), ownerId));
    }
    public void StopVoice(string ownerId)
    {
        if (_voiceSources.TryGetValue(ownerId, out var src))
        {
            src.Stop();
            _voiceSources.Remove(ownerId);
        }
    }
    public void PlayUI(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        var src = GetFreeSource();
        if (src == null) return;

        ConfigureSource(src, clip, AudioManager.Instance?.EnvironmentGroup, pitch);
        src.spatialBlend = 0f;
        src.volume = volumeScale;
        src.Play();
        StartCoroutine(ReturnAfterPlay(src, clip.length / Mathf.Max(pitch, 0.01f)));
    }
    private AudioSource GetFreeSource()
    {
        foreach (var s in _pool)
            if (!s.isPlaying) return s;

        Debug.LogWarning("[SoundPlayer] Пул звуков переполнен, добавляю новый источник.");
        var extra = CreatePooledSource($"PooledSound_extra_{_pool.Count}");
        _pool.Add(extra);
        return extra;
    }

    private static void ConfigureSource(AudioSource src, AudioClip clip, AudioMixerGroup group, float pitch = 1f)
    {
        src.clip = clip;
        src.loop = false;
        src.pitch = pitch;
        src.outputAudioMixerGroup = group;
    }

    private IEnumerator ReturnAfterPlay(AudioSource src, float duration, string voiceId = null)
    {
        yield return new WaitForSeconds(duration + 0.05f);
        src.Stop();
        src.clip = null;
        if (voiceId != null) _voiceSources.Remove(voiceId);
    }

    private AudioSource CreatePooledSource(string sourceName)
    {
        var go = new GameObject(sourceName);
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        return src;
    }
}