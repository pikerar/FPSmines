using UnityEngine;

/// <summary>
/// Ящик с флагами.
/// Скрипт для префаба ящика. При нажатии E вблизи - пополняет инвентарь.
/// </summary>
public class FlagBox : MonoBehaviour
{
    [Header("Кол-во флагов которое даёт ящик за одно взаимодействие")]
    public int flagsPerRefill = 5;
    // Мейби переработать на пополнение именно отсутствующего кол-ва флагов

    [Header("Дистанция взаимодействия (метры)")]
    public float interactDistance = 3f;
    
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
        }
    }

    void Update()
    {
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
            return;
        }

        int before = FlagInventory.Instance.CurrentFlags;
        int added = FlagInventory.Instance.AddFlags(flagsPerRefill);
        Debug.Log($"[FlagBox] было-{before} добавлено-{added} стало-{FlagInventory.Instance.CurrentFlags}");
    }
}