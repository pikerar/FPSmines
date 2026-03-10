using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD интерфейс: счётчик флагов + подсказки по управлению.
/// 
/// Настройка Canvas:
/// 1. Canvas (Screen Space — Overlay)
///    ├── FlagPanel (Image, угол экрана)
///    │   ├── FlagIcon (Image)
///    │   └── FlagCountText (TextMeshProUGUI) ← назначь в Inspector
///    └── HintText (TextMeshProUGUI) ← назначь в Inspector
/// 
/// Прикрепи этот скрипт на тот же Canvas или отдельный GameObject.
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
        // Переподписываемся в Start — к этому моменту все Awake уже выполнены
        // и FlagInventory.Instance гарантированно существует
        if (FlagInventory.Instance != null)
        {
            FlagInventory.Instance.onFlagsChanged.RemoveListener(UpdateFlagCount);
            FlagInventory.Instance.onFlagsChanged.AddListener(UpdateFlagCount);
            UpdateFlagCount(FlagInventory.Instance.CurrentFlags);
        }
        else
        {
            Debug.LogWarning("[MinefieldHUD] FlagInventory не найден! Проверь что GameManager с компонентом FlagInventory есть на сцене.");
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
            // Если флагов нет и хотим поставить — доп. подсказка
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

    // -------------------------------------------------------
    // Обновление счётчика флагов
    // -------------------------------------------------------
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