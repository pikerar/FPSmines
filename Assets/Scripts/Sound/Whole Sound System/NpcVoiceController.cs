using UnityEngine;

/// <summary>
/// Пример компонента голоса NPC.
/// Каждый NPC имеет уникальный ID — это позволяет AudioManager
/// прерывать его предыдущую реплику при воспроизведении новой.
/// </summary>
public class NpcVoiceController : MonoBehaviour
{
    [Header("Идентификатор NPC (должен быть уникальным на сцене)")]
    [SerializeField] private string npcId;

    [Header("Реплики")]
    [SerializeField] private AudioClip[] voiceLines;

    [Range(0f, 1f)]
    [SerializeField] private float volumeScale = 1f;

    private void Awake()
    {
        // Если ID не задан — генерируем из имени объекта + GUID
        if (string.IsNullOrEmpty(npcId))
            npcId = $"{gameObject.name}_{GetInstanceID()}";
    }

    /// <summary>Воспроизвести конкретную реплику по индексу.</summary>
    public void PlayLine(int index)
    {
        if (voiceLines == null || index < 0 || index >= voiceLines.Length) return;
        SoundPlayer.Instance?.PlayVoice(voiceLines[index], npcId, volumeScale);
    }

    /// <summary>Воспроизвести случайную реплику.</summary>
    public void PlayRandomLine()
    {
        if (voiceLines == null || voiceLines.Length == 0) return;
        int index = Random.Range(0, voiceLines.Length);
        PlayLine(index);
    }

    /// <summary>Остановить текущую реплику этого NPC.</summary>
    public void StopVoice()
    {
        SoundPlayer.Instance?.StopVoice(npcId);
    }
}
