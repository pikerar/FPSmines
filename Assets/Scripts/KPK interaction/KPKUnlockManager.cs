using UnityEngine;

public class KPKUnlockManager : MonoBehaviour
{
    [Header("Включить дебаг-логи")]
    [SerializeField] private bool debugLogs = true;

    public bool IsUnlocked { get; private set; } = false;
    public static KPKUnlockManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool CanUseKPK()
    {
        return IsUnlocked;
    }

    public void UnlockKPK()
    {
        if (IsUnlocked) return;

        IsUnlocked = true;
        if (debugLogs) Debug.Log("[KPKUnlock] ✅ КПК подобран! Tab разблокирован.");
    }
}