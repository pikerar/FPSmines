using UnityEngine;

public class Billboard3D : MonoBehaviour
{
    [Header("Слежение за камерой")]
    public Transform playerCamera;
    public bool facePlayer = true;    // переключатель в инспекторе

    void Update()
    {
        if (!facePlayer || playerCamera == null) return;

        transform.LookAt(
            transform.position + playerCamera.rotation * Vector3.forward,
            playerCamera.rotation * Vector3.up);
    }

    // Можно переключать и из кода
    public void SetFacePlayer(bool value) => facePlayer = value;
}