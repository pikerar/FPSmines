using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinefieldHUD : MonoBehaviour
{
    public static MinefieldHUD Instance { get; private set; }

    [Header("Счётчик флагов в инвентаре")]
    [SerializeField] private TextMeshProUGUI flagCountText;

    [Header("Счётчик мин на поле (появляется при входе в зону)")]
    [SerializeField] private GameObject mineCounterPanel;
    [SerializeField] private TextMeshProUGUI mineCountText;
    [SerializeField] private TextMeshProUGUI mineCounterLabel;
    [SerializeField] private TextMeshProUGUI fieldStatusText;

    [Header("Текст подсказки")]
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Тексты подсказок")]
    [SerializeField] private string mineHint = "ЛКМ — детонация  /  ПКМ — поставить флаг";
    [SerializeField] private string mineFlaggedHint = "ЛКМ — убрать флаг";
    [SerializeField] private string mineNoFlagsHint = "ЛКМ — детонация  /  Флагов нет";
    [SerializeField] private string boxHint = "E — забрать флаги";
    [SerializeField] private string boxEmptyHint = "Флагов нет";
    [SerializeField] private string buttonHint = "E — взаимодействие";
    [SerializeField] private string buttonUsedHint = "";

    [Header("Цвета счётчика мин")]
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorError = Color.red;
    [SerializeField] private Color colorCleared = Color.green;

    private Minefield currentField;

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

        if (mineCounterPanel != null) mineCounterPanel.SetActive(false);
        SetHint("");
    }

    // -------------------------------------------------------
    // Счётчик мин — вызывается MinefieldZone
    // -------------------------------------------------------

    public void ShowMineCounter(Minefield field)
    {
        currentField = field;

        if (mineCounterPanel != null)
        {
            mineCounterPanel.SetActive(true);
            Debug.Log("[MinefieldHUD] MineCounterPanel активирован");
        }

        UpdateMineCounter(field);
    }

    public void HideMineCounter()
    {
        currentField = null;
        if (mineCounterPanel != null) mineCounterPanel.SetActive(false);
    }

    /// <summary>
    /// Вызывается из Minefield.CheckProgress() при каждом изменении
    /// </summary>
    public void UpdateMineCounter(Minefield field)
    {
        if (field != currentField) return;
        if (mineCountText == null) return;

        int flagged = field.TotalFlaggedCells;
        int total = field.TotalDangerousMines;

        mineCountText.text = $"{flagged} / {total}";

        if (field.IsCleared)
        {
            mineCountText.color = colorCleared;
            if (fieldStatusText != null) { fieldStatusText.text = "Решено!"; fieldStatusText.color = colorCleared; }
        }
        else if (flagged > total)
        {
            mineCountText.color = colorError;
            if (fieldStatusText != null) { fieldStatusText.text = "Ошибка!"; fieldStatusText.color = colorError; }
        }
        else
        {
            mineCountText.color = colorNormal;
            if (fieldStatusText != null) fieldStatusText.text = "";
        }
    }

    // -------------------------------------------------------
    // Подсказки при наведении
    // -------------------------------------------------------

    public void UpdateHoverHint(MineCell mine, FlagBox box, BarrierButton button = null)
    {
        if (mine != null && !mine.isRevealed)
        {
            if (mine.isFlagged)
                SetHint(mineFlaggedHint);
            else
            {
                bool hasFlags = FlagInventory.Instance != null && FlagInventory.Instance.CurrentFlags > 0;
                SetHint(hasFlags ? mineHint : mineNoFlagsHint);
            }
        }
        else if (box != null)
            SetHint(box.FlagsRemaining > 0 ? boxHint : boxEmptyHint);
        else if (button != null)
            SetHint(button.IsActivated ? buttonUsedHint : buttonHint);
        else
            SetHint("");
    }

    // -------------------------------------------------------
    // Вспомогательные
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