using System.Collections.Generic;
using UnityEngine;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance { get; private set; }

    // Хранилище: ID объекта → его состояние
    private Dictionary<string, object> savedStates = new Dictionary<string, object>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Вызывается при достижении чекпоинта
    public void CaptureScene()
    {
        savedStates.Clear();
        var saveables = FindObjectsByType<SaveableObject>(FindObjectsSortMode.None);

        foreach (var obj in saveables)
        {
            savedStates[obj.GetUniqueID()] = obj.CaptureState();
        }

        Debug.Log($"Scene captured: {savedStates.Count} objects saved");
    }

    // Вызывается при загрузке сцены
    public void RestoreScene()
    {
        if (savedStates.Count == 0) return;

        var saveables = FindObjectsByType<SaveableObject>(FindObjectsSortMode.None);

        foreach (var obj in saveables)
        {
            if (savedStates.TryGetValue(obj.GetUniqueID(), out object state))
            {
                obj.RestoreState(state);
            }
        }

        Debug.Log("Scene restored");
    }

    public void ClearState()
    {
        savedStates.Clear();
    }

    public bool HasSavedState() => savedStates.Count > 0;
}