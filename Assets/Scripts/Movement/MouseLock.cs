using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;

    [Header("Clamp")]
    public float minY = -85f;
    public float maxY = 85f;

    [Header("References")]
    public Transform camHolder;   // перетащи cam holder
    public Transform camPoint;    // перетащи CamPoint

    private float rotX = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY;

        // Player крутится по горизонтали
        transform.Rotate(0f, mouseX, 0f);

        // cam holder наклоняется по вертикали
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minY, maxY);
        camHolder.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    }
}