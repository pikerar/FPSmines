using UnityEngine;

public class InteractionSoundPlayer : MonoBehaviour
{
    [System.Serializable]
    public class SoundEntry
    {
        public string key;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float pitch = 1f;
    }

    [Header("Источник звука (если не назначен — создаётся автоматически)")]
    [SerializeField] private AudioSource audioSource;

    [Header("Пул звуков")]
    [SerializeField] private SoundEntry[] sounds;

    void Awake()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    public void Play(string key)
    {
        foreach (var entry in sounds)
        {
            if (entry.key == key && entry.clip != null)
            {
                audioSource.pitch = entry.pitch;
                audioSource.PlayOneShot(entry.clip, entry.volume);
                return;
            }
        }

        Debug.LogWarning($"[InteractionSoundPlayer] Звук с ключом '{key}' не найден");
    }
}