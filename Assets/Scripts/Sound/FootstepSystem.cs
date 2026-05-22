using UnityEngine;

public class FootstepSystem : MonoBehaviour
{
    [Header("Foot Bones")]
    [Tooltip("Drag the left foot bone transform from your rig")]
    public Transform leftFootBone;

    [Tooltip("Drag the right foot bone transform from your rig")]
    public Transform rightFootBone;

    [Header("Raycast Settings")]
    [Tooltip("How far down the ray reaches (should exceed max leg extension)")]
    public float rayLength = 1.5f;

    [Tooltip("Layer mask for ground objects. Set to 'Default' or create a 'Ground' layer")]
    public LayerMask groundLayerMask = ~0; 

    [Tooltip("Distance threshold: foot is considered 'planted' when ray hit distance <= this value")]
    [Range(0.01f, 0.5f)]
    public float stepThreshold = 0.12f;

    [Tooltip("Foot must rise at least this much above stepThreshold before the next step fires")]
    [Range(0.01f, 0.3f)]
    public float resetHysteresis = 0.06f;

    [Header("Audio")]
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

    private bool _leftStepped = false;
    private bool _rightStepped = false;

    private void Update()
    {
        if (leftFootBone != null) ProcessFoot(leftFootBone, ref _leftStepped);
        if (rightFootBone != null) ProcessFoot(rightFootBone, ref _rightStepped);
    }

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
                stepped = true;
                PlayFootstep(hit, foot.position);
            }
            else if (stepped && distance > stepThreshold + resetHysteresis)
            {
                stepped = false;
            }
        }
        else
        {
            stepped = false;
        }
    }

    private void PlayFootstep(RaycastHit hit, Vector3 footWorldPos)
    {
        if (SoundPlayer.Instance == null) return;

        AudioClip[] clips = GetClipsForSurface(hit.collider);
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        float pitch = Random.Range(pitchRange.x, pitchRange.y);

        SoundPlayer.Instance.PlayEnvironment(clip, hit.point, volume, pitch);
    }

    private AudioClip[] GetClipsForSurface(Collider col)
    {
        FootstepSurface surface = col.GetComponentInParent<FootstepSurface>();

        if (surface == null)
            return fallbackSounds;

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

    public void OnFootstepEvent(Vector3 worldPosition)
    {
        Ray ray = new Ray(worldPosition + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, rayLength + 0.1f, groundLayerMask))
            PlayFootstep(hit, worldPosition);
    }
}