using UnityEngine;

/// <summary>
/// Пример компонента шагов для персонажа/NPC.
/// Вешается на GameObject с CharacterController или Rigidbody.
///
/// ИСПОЛЬЗОВАНИЕ:
/// — Вызывай PlayFootstep() из анимационного события, либо по таймеру в Update.
/// — Заполни массив clips несколькими вариантами звука шагов (будет случайный выбор).
/// </summary>
public class FootstepController : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Настройки")]
    [Range(0f, 1f)]
    [SerializeField] private float volumeScale = 1f;

    [Tooltip("Минимальная скорость движения, при которой играют шаги (если используешь автотаймер)")]
    [SerializeField] private float minSpeedThreshold = 0.1f;

    [Tooltip("Интервал между шагами в секундах (для режима AutoTimer)")]
    [SerializeField] private float stepInterval = 0.45f;

    [Header("Режим воспроизведения")]
    [Tooltip("true — шаги играют автоматически по таймеру при движении; false — только через PlayFootstep()")]
    [SerializeField] private bool useAutoTimer = false;

    private CharacterController _cc;
    private float _stepTimer;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!useAutoTimer) return;
        if (_cc == null) return;

        bool isMoving = _cc.velocity.magnitude > minSpeedThreshold && _cc.isGrounded;
        if (!isMoving) return;

        _stepTimer -= Time.deltaTime;
        if (_stepTimer <= 0f)
        {
            PlayFootstep();
            _stepTimer = stepInterval;
        }
    }

    /// <summary>
    /// Вызывай из Animation Event или вручную.
    /// </summary>
    public void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        if (SoundPlayer.Instance == null) return;

        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        SoundPlayer.Instance.PlayEnvironment(clip, transform.position, volumeScale);
    }
}
