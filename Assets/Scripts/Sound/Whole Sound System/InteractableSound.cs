using UnityEngine;

/// <summary>
/// Компонент для звуков взаимодействия с объектом (дверь, рычаг, предмет и т.д.)
/// Вешается на любой интерактивный объект.
///
/// ИСПОЛЬЗОВАНИЕ:
///   // В своём скрипте взаимодействия:
///   GetComponent<InteractableSound>()?.PlayInteract();
/// </summary>
public class InteractableSound : MonoBehaviour
{
    [Header("Звуки")]
    [Tooltip("Звук при взаимодействии (нажать, поднять, активировать)")]
    [SerializeField] private AudioClip interactClip;

    [Tooltip("Звук при отмене/закрытии (опционально)")]
    [SerializeField] private AudioClip cancelClip;

    [Range(0f, 1f)]
    [SerializeField] private float volumeScale = 1f;

    public void PlayInteract()
    {
        if (interactClip == null) return;
        SoundPlayer.Instance?.PlayEnvironment(interactClip, transform.position, volumeScale);
    }

    public void PlayCancel()
    {
        if (cancelClip == null) return;
        SoundPlayer.Instance?.PlayEnvironment(cancelClip, transform.position, volumeScale);
    }
}
