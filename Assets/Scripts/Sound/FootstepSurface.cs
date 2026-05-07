using UnityEngine;

public class FootstepSurface : MonoBehaviour
{
    public enum SurfaceType
    {
        Stone,
        Dirt,
        Grass,
        Wood,
        Metal
    }

    [Tooltip("Surface type that defines which footstep sounds will play")]
    public SurfaceType surfaceType = SurfaceType.Dirt;
}