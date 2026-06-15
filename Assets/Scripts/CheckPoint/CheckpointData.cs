[System.Serializable]
public class CheckpointData
{
    public float posX, posY, posZ;
    public int currentFlags;
    public string sceneName;

    public UnityEngine.Vector3 Position => new UnityEngine.Vector3(posX, posY, posZ);
}