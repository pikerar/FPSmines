using UnityEngine;

public class FlagBox : MonoBehaviour
{
    [Header("Флаги внутри ящика")]
    [SerializeField] private int flagsInBox = 5;
    [SerializeField] private float interactDistance = 3f;

    // Дочерние объекты-префабы флагов внутри ящика
    // Заполни в Inspector перетащив объекты флагов из Hierarchy
    [SerializeField] private GameObject[] flagObjects;

    public bool IsPlayerNearby { get; private set; } = false;
    public int FlagsRemaining => flagsRemaining;

    private Transform playerTransform;
    private int flagsRemaining;

    void Start()
    {
        flagsRemaining = flagsInBox;
        TryFindPlayer();
        UpdateFlagVisuals();
    }

    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("[FlagBox] Игрок не найден! Убедись что тег 'Player' стоит на игроке.");
    }

    void Update()
    {
        if (playerTransform == null) { TryFindPlayer(); return; }

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        IsPlayerNearby = dist <= interactDistance;

        if (IsPlayerNearby && Input.GetKeyDown(KeyCode.E))
            Refill();
    }

    void Refill()
    {
        if (FlagInventory.Instance == null) { Debug.LogWarning("[FlagBox] FlagInventory не найден!"); return; }
        if (flagsRemaining <= 0) { Debug.Log("[FlagBox] Ящик пуст!"); return; }

        int added = FlagInventory.Instance.AddFlags(flagsRemaining);
        flagsRemaining -= added;

        UpdateFlagVisuals();
        Debug.Log($"[FlagBox] Отдано флагов: {added}, осталось в ящике: {flagsRemaining}");
    }

    void UpdateFlagVisuals()
    {
        if (flagObjects == null || flagObjects.Length == 0) return;

        int totalObjects = flagObjects.Length;
        int shouldBeVisible = flagsInBox > 0
            ? Mathf.RoundToInt((float)flagsRemaining / flagsInBox * totalObjects)
            : 0;

        for (int i = 0; i < totalObjects; i++)
        {
            if (flagObjects[i] == null) continue;

            if (i < shouldBeVisible)
                flagObjects[i].SetActive(true);
            else
                Destroy(flagObjects[i]);
        }
    }
}