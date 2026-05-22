using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AmbientController : MonoBehaviour
{
    [System.Serializable]
    public struct SceneAmbient
    {
        public string sceneName; 
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Header("Ambient Tracks")]
    [SerializeField] private SceneAmbient[] sceneAmbients;

    [Header("Crossfade")]
    [SerializeField] private float fadeDuration = 1.5f;

    private AudioSource _sourceA;
    private AudioSource _sourceB;
    private bool _usingA = true;

    private AudioSource Active   => _usingA ? _sourceA : _sourceB;
    private AudioSource Inactive => _usingA ? _sourceB : _sourceA;

    private void Awake()
    {
        _sourceA = CreateSource("Ambient_A");
        _sourceB = CreateSource("Ambient_B");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        _sourceA.outputAudioMixerGroup = AudioManager.Instance?.MusicGroup;
        _sourceB.outputAudioMixerGroup = AudioManager.Instance?.MusicGroup;
        // Воспроизвести эмбиент для стартовой сцены без фейда
        PlayForScene(SceneManager.GetActiveScene().name, fade: false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name, fade: true);
    }

    private void PlayForScene(string sceneName, bool fade)
    {
        SceneAmbient? match = null;
        foreach (var entry in sceneAmbients)
        {
            if (entry.sceneName == sceneName)
            {
                match = entry;
                break;
            }
        }

        AudioClip nextClip   = match?.clip;
        float     nextVolume = match?.volume ?? 1f;

        if (Active.clip == nextClip && Active.isPlaying) return;

        if (fade)
            StartCoroutine(Crossfade(nextClip, nextVolume));
        else
            PlayImmediate(nextClip, nextVolume);
    }

    private void PlayImmediate(AudioClip clip, float volume)
    {
        Active.clip   = clip;
        Active.volume = volume;
        if (clip != null) Active.Play();
        else              Active.Stop();
    }

    private IEnumerator Crossfade(AudioClip nextClip, float targetVolume)
    {
        AudioSource outgoing = Active;
        _usingA = !_usingA;
        AudioSource incoming = Active; 

        incoming.clip   = nextClip;
        incoming.volume = 0f;
        if (nextClip != null) incoming.Play();

        float elapsed = 0f;
        float startVolume = outgoing.volume;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            outgoing.volume = Mathf.Lerp(startVolume, 0f, t);
            if (nextClip != null)
                incoming.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        outgoing.Stop();
        outgoing.clip = null;
        if (nextClip != null) incoming.volume = targetVolume;
    }

    private AudioSource CreateSource(string sourceName)
    {
        var go = new GameObject(sourceName);
        go.transform.SetParent(transform);

        var src = go.AddComponent<AudioSource>();
        src.loop       = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D

        if (AudioManager.Instance != null)
            src.outputAudioMixerGroup = AudioManager.Instance.MusicGroup;

        return src;
    }
}
