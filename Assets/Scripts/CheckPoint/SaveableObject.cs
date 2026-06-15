using UnityEngine;

public class SaveableObject : MonoBehaviour, ISaveable
{
    [SerializeField] private string uniqueID;
    [SerializeField] private bool useObjectName = false;

    public string GetUniqueID() => useObjectName ? gameObject.name : uniqueID;

    [System.Serializable]
    public struct State
    {
        public bool isActive;
        public float posX, posY, posZ;
        public float rotX, rotY, rotZ;
    }

    public object CaptureState()
    {
        return new State
        {
            isActive = gameObject.activeSelf,
            posX = transform.position.x,
            posY = transform.position.y,
            posZ = transform.position.z,
            rotX = transform.eulerAngles.x,
            rotY = transform.eulerAngles.y,
            rotZ = transform.eulerAngles.z
        };
    }

    public void RestoreState(object state)
    {
        var s = (State)state;
        gameObject.SetActive(s.isActive);
        transform.position = new Vector3(s.posX, s.posY, s.posZ);
        transform.eulerAngles = new Vector3(s.rotX, s.rotY, s.rotZ);
    }
}