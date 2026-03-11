using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD интерфейс
/// </summary>
public class MinefieldHUD : MonoBehaviour
{
    public static MinefieldHUD Instance { get; private set; }

    [Header("Счётчик флагов")]
    public TextMeshProUGUI flagCountText;

    [Header("Текст подсказки (по центру низа или где угодно)")]
    public TextMeshProUGUI hintText;

    [Header("Тексты подсказок")]
    public string mineHint = "ЛКМ — детонация  /  ПКМ — поставить флаг";
    public string boxHint = "E — пополнить флаги";
    public string noFlagsHint = "Флагов нет! Найди ящик с флагами";

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

    public void UpdateHoverHint(MineCell mine, FlagBox box)
    {
        if (mine != null && !mine.isRevealed)
        {
            bool hasFlags = FlagInventory.Instance != null && FlagInventory.Instance.CurrentFlags > 0;
            if (!hasFlags && !mine.isFlagged)
                SetHint($"ЛКМ — детонация  /  {noFlagsHint}");
            else
                SetHint(mineHint);
        }
        else if (box != null)
        {
            SetHint(boxHint);
        }
        else
        {
            SetHint("");
        }
    }

    void UpdateFlagCount(int count)
    {
        if (flagCountText != null)
            flagCountText.text = $"Flags: {count}";
    }

    void SetHint(string text)
    {
        if (hintText == null) return;
        hintText.text = text;
        hintText.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }
}