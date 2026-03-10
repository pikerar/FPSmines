using UnityEngine;

/// <summary>
/// Ящик с флагами.
/// Прикрепи к префабу ящика. При нажатии E вблизи — пополняет инвентарь.
/// </summary>
public class FlagBox : MonoBehaviour
{
    [Header("Кол-во флагов которое даёт ящик за одно взаимодействие")]
    public int flagsPerRefill = 5;

    [Header("Дистанция взаимодействия (метры)")]
    public float interactDistance = 3f;

    // Ящик будет доступен для наведения PlayerInteraction
    public bool IsPlayerNearby { get; private set; } = false;

    private Transform playerTransform;

    void Start()
    {
        TryFindPlayer();
    }

    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("[FlagBox] Игрок найден: " + player.name);
        }
        else
        {
            Debug.LogWarning("[FlagBox] Игрок НЕ найден! Убедись что у игрока тег 'Player'");
        }
    }

    void Update()
    {
        // Если ещё не нашли игрока — пробуем снова
        if (playerTransform == null)
        {
            TryFindPlayer();
            return;
        }

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        IsPlayerNearby = dist <= interactDistance;

        if (IsPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Refill();
        }
    }

    void Refill()
    {
        if (FlagInventory.Instance == null)
        {
            Debug.LogWarning("[FlagBox] FlagInventory не найден на сцене!");
            return;
        }

        int before = FlagInventory.Instance.CurrentFlags;
        int added = FlagInventory.Instance.AddFlags(flagsPerRefill);
        Debug.Log($"[FlagBox] Было: {before}, добавлено: {added}, стало: {FlagInventory.Instance.CurrentFlags}");
    }
}