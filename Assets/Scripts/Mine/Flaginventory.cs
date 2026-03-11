using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Синглтон инвентаря флагов
/// Накинуть на геймменеджер и усё
/// </summary>
public class FlagInventory : MonoBehaviour
{
    public static FlagInventory Instance { get; private set; }

    [Header("Начальное количество флагов")]
    public int startingFlags = 2;

    [Header("Максимальный запас")]
    public int maxFlags = 5;

    public int CurrentFlags { get; private set; }

    public UnityEvent<int> onFlagsChanged = new UnityEvent<int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CurrentFlags = startingFlags;
    }

    /// <summary>
    /// потратить 1 флаг, возврат тру при успехе
    /// </summary>
    public bool UseFlag()
    {
        if (CurrentFlags <= 0) return false;
        CurrentFlags--;
        onFlagsChanged.Invoke(CurrentFlags);
        return true;
    }

    /// <summary>
    /// вернуть 1 флаг снятый с мины, но не больше максимума
    /// </summary>
    public void ReturnFlag()
    {
        CurrentFlags = Mathf.Min(CurrentFlags + 1, maxFlags);
        onFlagsChanged.Invoke(CurrentFlags);
    }

    /// <summary>
    /// пополнение флагов из ящика, но не больше максимума
    /// </summary>
    public int AddFlags(int amount)
    {
        int before = CurrentFlags;
        CurrentFlags = Mathf.Min(CurrentFlags + amount, maxFlags);
        int added = CurrentFlags - before;
        onFlagsChanged.Invoke(CurrentFlags);
        return added;
    }
}