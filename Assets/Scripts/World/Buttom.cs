using UnityEngine;
using System.Collections;

public class BarrierButton : MonoBehaviour
{
    [Header("Шлагбаум")]
    [SerializeField] private Barrier barrier;

    [Header("Задержка перед подъёмом (сек)")]
    [SerializeField] private float delayBeforeOpen = 3f;

    [Header("Дистанция взаимодействия")]
    [SerializeField] private float interactDistance = 2f;

    [Header("Реплика перед открытием")]
    [SerializeField] private AudioClip voiceLine;
    [SerializeField] private AudioSource voiceSource; 
    [Range(0f, 1f)]
    [SerializeField] private float voiceVolume = 1f;

    public bool IsActivated { get; private set; } = false;
    public event System.Action OnActivated; 

    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (voiceSource == null)
        {
            voiceSource = gameObject.AddComponent<AudioSource>();
            voiceSource.spatialBlend = 1f;      
            voiceSource.rolloffMode = AudioRolloffMode.Linear;
            voiceSource.minDistance = 1f;
            voiceSource.maxDistance = 15f;
            voiceSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (IsActivated) return;
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        bool nearby = dist <= interactDistance;

        if (nearby && InputHandler.Instance != null && InputHandler.Instance.InteractPressed)
            Activate();
    }

    void Activate()
    {
        IsActivated = true;
        OnActivated?.Invoke();
        StartCoroutine(OpenSequence());
    }

    IEnumerator OpenSequence()
    {
        if (voiceLine != null && voiceSource != null)
        {
            voiceSource.PlayOneShot(voiceLine, voiceVolume);
            yield return new WaitForSeconds(Mathf.Max(voiceLine.length, delayBeforeOpen));
        }
        else
        {
            yield return new WaitForSeconds(delayBeforeOpen);
        }

        barrier?.Open();
    }
}