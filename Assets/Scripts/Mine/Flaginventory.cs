using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Синглтон инвентаря флагов.
/// Помести на любой постоянный GameObject (например GameManager).
/// </summary>
public class FlagInventory : MonoBehaviour
{
    public static FlagInventory Instance { get; private set; }

    [Header("Начальное количество флагов")]
    public int startingFlags = 5;

    [Header("Максимальный запас")]
    public int maxFlags = 20;

    public int CurrentFlags { get; private set; }

    // Событие для UI — подписывается HUD
    public UnityEvent<int> onFlagsChanged = new UnityEvent<int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Инициализируем в Awake — до любого Start в других скриптах
        CurrentFlags = startingFlags;
    }

    /// <summary>
    /// Потратить 1 флаг. Возвращает true если успешно.
    /// </summary>
    public bool UseFlag()
    {
        if (CurrentFlags <= 0) return false;
        CurrentFlags--;
        onFlagsChanged.Invoke(CurrentFlags);
        return true;
    }

    /// <summary>
    /// Вернуть 1 флаг (когда снимают флаг с мины).
    /// </summary>
    public void ReturnFlag()
    {
        CurrentFlags = Mathf.Min(CurrentFlags + 1, maxFlags);
        onFlagsChanged.Invoke(CurrentFlags);
    }

    /// <summary>
    /// Пополнить из ящика. Возвращает реально добавленное кол-во.
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