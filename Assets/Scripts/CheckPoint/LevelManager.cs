using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Vector3 SpawnLoc;
    private const string SaveKey = "CheckpointSave";

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Вызывается из CheckPoint.cs
    public void SaveCheckpoint(Vector3 position)
    {
        CheckpointData data = new CheckpointData
        {
            posX = position.x,
            posY = position.y,
            posZ = position.z,
            currentFlags = FlagInventory.Instance.CurrentFlags, // просто берём напрямую
            sceneName = SceneManager.GetActiveScene().name
        };

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    public CheckpointData LoadCheckpoint()
    {
        if (!HasSave()) return null;

        string json = PlayerPrefs.GetString(SaveKey);
        return JsonUtility.FromJson<CheckpointData>(json);
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }
}