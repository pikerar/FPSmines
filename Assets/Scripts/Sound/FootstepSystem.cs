using UnityEngine;
public class FootstepSystem : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────
    //  Inspector fields
    // ─────────────────────────────────────────────────────────────

    [Header("Foot Bones")]
    [Tooltip("Drag the left foot bone transform from your rig")]
    public Transform leftFootBone;

    [Tooltip("Drag the right foot bone transform from your rig")]
    public Transform rightFootBone;

    [Header("Raycast Settings")]
    [Tooltip("How far down the ray reaches (should exceed max leg extension)")]
    public float rayLength = 1.5f;

    [Tooltip("Layer mask for ground objects. Set to 'Default' or create a 'Ground' layer")]
    public LayerMask groundLayerMask = ~0; // ~0 = everything

    [Tooltip("Distance threshold: foot is considered 'planted' when ray hit distance <= this value")]
    [Range(0.01f, 0.5f)]
    public float stepThreshold = 0.12f;

    [Tooltip("Foot must rise at least this much above stepThreshold before the next step fires")]
    [Range(0.01f, 0.3f)]
    public float resetHysteresis = 0.06f;

    [Header("Audio")]
    [Tooltip("AudioSource used to play footstep sounds")]
    public AudioSource audioSource;

    [Range(0f, 1f)]
    public float volume = 0.8f;

    [Tooltip("Random pitch range applied per step (1 = no variation)")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    [Header("Surface Sounds — Stone")]
    public AudioClip[] stoneSounds;

    [Header("Surface Sounds — Dirt")]
    public AudioClip[] dirtSounds;

    [Header("Surface Sounds — Grass")]
    public AudioClip[] grassSounds;

    [Header("Surface Sounds — Wood")]
    public AudioClip[] woodSounds;

    [Header("Surface Sounds — Metal")]
    public AudioClip[] metalSounds;

    [Header("Fallback Sound")]
    [Tooltip("Played when no FootstepSurface component is found on the hit object")]
    public AudioClip[] fallbackSounds;

    [Header("Debug")]
    [Tooltip("Draw rays in the Scene view while playing")]
    public bool drawDebugRays = true;

    // ─────────────────────────────────────────────────────────────
    //  Private state
    // ─────────────────────────────────────────────────────────────

    // Per-foot state machine
    private bool _leftStepped = false;
    private bool _rightStepped = false;

    // ─────────────────────────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        // Auto-find AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (leftFootBone != null) ProcessFoot(leftFootBone, ref _leftStepped);
        if (rightFootBone != null) ProcessFoot(rightFootBone, ref _rightStepped);
    }

    // ─────────────────────────────────────────────────────────────
    //  Core logic
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Casts a ray downward from the foot bone.
    /// Triggers a footstep sound when the foot is close enough to the ground,
    /// and resets the trigger when the foot lifts back up.
    /// </summary>
    private void ProcessFoot(Transform foot, ref bool stepped)
    {
        Vector3 origin = foot.position;
        Ray ray = new Ray(origin, Vector3.down);

        if (drawDebugRays)
            Debug.DrawRay(origin, Vector3.down * rayLength, stepped ? Color.red : Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, groundLayerMask))
        {
            float distance = hit.distance;

            if (!stepped && distance <= stepThreshold)
            {
                // ── Foot has just touched / nearly touched the ground ──
                stepped = true;
                PlayFootstep(hit);
            }
            else if (stepped && distance > stepThreshold + resetHysteresis)
            {
                // ── Foot has lifted: allow next step ──
                stepped = false;
            }
        }
        else
        {
            // No ground beneath — reset so we don't miss the next landing
            stepped = false;
        }
    }

    /// <summary>
    /// Selects the correct sound array based on the FootstepSurface
    /// component found on the hit object, then plays a random clip.
    /// </summary>
    private void PlayFootstep(RaycastHit hit)
    {
        AudioClip[] clips = GetClipsForSurface(hit.collider);

        if (clips == null || clips.Length == 0)
            return;

        // Pick a random clip
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Reads FootstepSurface from the hit collider (or its parent)
    /// and returns the matching AudioClip array.
    /// </summary>
    private AudioClip[] GetClipsForSurface(Collider col)
    {
        // Look for FootstepSurface on the object itself, then walk up the hierarchy
        FootstepSurface surface = col.GetComponentInParent<FootstepSurface>();

        if (surface == null)
        {
            // No component found — use fallback
            return fallbackSounds;
        }

        switch (surface.surfaceType)
        {
            case FootstepSurface.SurfaceType.Stone: return stoneSounds;
            case FootstepSurface.SurfaceType.Dirt: return dirtSounds;
            case FootstepSurface.SurfaceType.Grass: return grassSounds;
            case FootstepSurface.SurfaceType.Wood: return woodSounds;
            case FootstepSurface.SurfaceType.Metal: return metalSounds;
            default: return fallbackSounds;
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  Public API (call from other scripts if needed)
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Manually fire a footstep at a world position (useful for animation events).
    /// </summary>
    public void OnFootstepEvent(Vector3 worldPosition)
    {
        Ray ray = new Ray(worldPosition + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, rayLength + 0.1f, groundLayerMask))
            PlayFootstep(hit);
    }
}