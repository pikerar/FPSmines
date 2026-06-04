using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Play3DSound : MonoBehaviour
{
    [SerializeField] private AudioClip soundClip;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = false;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Настройка 3D звука
        audioSource.spatialBlend = 1f;        // Полностью 3D
        audioSource.spatialize = true;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (soundClip != null)
        {
            audioSource.clip = soundClip;
            audioSource.volume = volume;
            audioSource.loop = loop;

            if (playOnStart)
            {
                PlaySound();
            }
        }
        else
        {
            Debug.LogWarning("Звуковой клип не назначен на объекте: " + gameObject.name);
        }
    }

    public void PlaySound()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}