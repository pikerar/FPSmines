using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD интерфейс: счётчик флагов + подсказки по управлению.
/// </summary>
public class MinefieldHUD : MonoBehaviour
{
    public static MinefieldHUD Instance { get; private set; }

    [Header("Счётчик флагов")]
    public TextMeshProUGUI flagCountText;

    [Header("Текст подсказки (по центру низа или где угодно)")]
    public TextMeshProUGUI hintText;

    [Header("Тексты подсказок")]
    [SerializeField] private string mineHint = "ЛКМ — детонация  /  ПКМ — поставить флаг";
    [SerializeField] private string mineFlaggedHint = "ЛКМ — убрать флаг";
    [SerializeField] private string mineNoFlagsHint = "ЛКМ — детонация  /  Флагов нет";
    [SerializeField] private string boxHint = "E — забрать флаги";
    [SerializeField] private string boxEmptyHint = "Флагов нет";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
        if (FlagInventory.Instance != null)
            FlagInventory.Instance.onFlagsChanged.AddListener(UpdateFlagCount);
    }

    void OnDisable()
    {
        if (FlagInventory.Instance != null)
            FlagInventory.Instance.onFlagsChanged.RemoveListener(UpdateFlagCount);
    }

    void Start()
    {
        if (FlagInventory.Instance != null)
        {
            FlagInventory.Instance.onFlagsChanged.RemoveListener(UpdateFlagCount);
            FlagInventory.Instance.onFlagsChanged.AddListener(UpdateFlagCount);
            UpdateFlagCount(FlagInventory.Instance.CurrentFlags);
        }
        SetHint("");
    }

    // -------------------------------------------------------
    // Вызывается PlayerInteraction каждый кадр
    // -------------------------------------------------------
    public void UpdateHoverHint(MineCell mine, FlagBox box)
    {
        if (mine != null && !mine.isRevealed)
        {
            if (mine.isFlagged)
            {
                // На мине стоит флаг - только убрать
                SetHint(mineFlaggedHint);
            }
            else
            {
                // Флагов в инвентаре нет — нельзя поставить
                bool hasFlags = FlagInventory.Instance != null && FlagInventory.Instance.CurrentFlags > 0;
                SetHint(hasFlags ? mineHint : mineNoFlagsHint);
            }
        }
        else if (box != null)
        {
            SetHint(box.FlagsRemaining > 0 ? boxHint : boxEmptyHint);
        }
        else
        {
            SetHint("");
        }
    }

    // -------------------------------------------------------
    // Обновление счётчика флагов
    // -------------------------------------------------------
    void UpdateFlagCount(int count)
    {
        if (flagCountText != null)
            flagCountText.text = $"Флаги: {count}";
    }

    void SetHint(string text)
    {
        if (hintText == null) return;
        hintText.text = text;
        hintText.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }
}