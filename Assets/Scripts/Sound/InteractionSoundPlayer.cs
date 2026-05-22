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

    [Header("Пул звуков")]
    [SerializeField] private SoundEntry[] sounds;

    public void Play(string key)
    {
        var entry = FindEntry(key);
        if (entry == null)
        {
            Debug.LogWarning($"[InteractionSoundPlayer] Звук с ключом '{key}' не найден.");
            return;
        }

        SoundPlayer.Instance?.PlayEnvironment(entry.clip, transform.position, entry.volume, entry.pitch);
    }


    public void PlayUI(string key)
    {
        var entry = FindEntry(key);
        if (entry == null)
        {
            Debug.LogWarning($"[InteractionSoundPlayer] Звук с ключом '{key}' не найден.");
            return;
        }

        SoundPlayer.Instance?.PlayUI(entry.clip, entry.volume, entry.pitch);
    }


    private SoundEntry FindEntry(string key)
    {
        if (sounds == null) return null;
        foreach (var entry in sounds)
            if (entry.key == key && entry.clip != null)
                return entry;
        return null;
    }
}