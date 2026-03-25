using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Minefield : MonoBehaviour
{
    [Header("Настройка поля")]
    [SerializeField] private bool randomizeMines = false;
    [SerializeField] private int mineCount = 10;

    [Header("Радиус соседства (единицы Unity)")]
    [SerializeField] private float neighborRadius = 1.6f;

    // Событие — поле решено
    public UnityEvent onFieldCleared = new UnityEvent();

    // Публичный прогресс для HUD
    public int TotalDangerousMines { get; private set; }
    public int FlaggedDangerousMines { get; private set; } // правильно отмеченные (для победы)
    public int TotalFlaggedCells { get; private set; }     // все флаги на поле (для счётчика HUD)
    public bool IsCleared { get; private set; } = false;

    private List<MineCell> cells = new List<MineCell>();

    void Awake()
    {
        CollectCells();
        if (randomizeMines) PlaceRandomMines();
        CalculateValues();
        CountDangerousMines();
    }

    void CollectCells()
    {
        cells.Clear();
        MineCell[] found = GetComponentsInChildren<MineCell>();
        cells.AddRange(found);
        Debug.Log($"[Minefield] найдено ячеек: {cells.Count}");
    }

    void PlaceRandomMines()
    {
        foreach (var c in cells) c.SetValue(0);

        int placed = 0, attempts = 0, maxAttempts = cells.Count * 10;
        while (placed < mineCount && attempts < maxAttempts)
        {
            attempts++;
            int idx = Random.Range(0, cells.Count);
            if (cells[idx].value != 9) { cells[idx].SetValue(9); placed++; }
        }
        Debug.Log($"[Minefield] расставлено мин: {placed}");
    }

    void CalculateValues()
    {
        foreach (var cell in cells)
        {
            if (cell.value == 9) continue;
            int count = 0;
            foreach (var neighbor in GetNeighbors(cell))
                if (neighbor.value == 9) count++;
            cell.SetValue(count);
        }
    }

    void CountDangerousMines()
    {
        TotalDangerousMines = 0;
        foreach (var cell in cells)
            if (cell.value == 9) TotalDangerousMines++;
    }

    List<MineCell> GetNeighbors(MineCell cell)
    {
        List<MineCell> result = new List<MineCell>();
        foreach (var other in cells)
        {
            if (other == cell) continue;
            if (Vector3.Distance(cell.transform.position, other.transform.position) <= neighborRadius)
                result.Add(other);
        }
        return result;
    }

    public void OnLeftClick(MineCell cell)
    {
        if (cell.isRevealed) return;

        if (cell.isFlagged)
        {
            cell.ToggleFlag();
            FlagInventory.Instance?.ReturnFlag();
            CheckProgress();
            return;
        }

        cell.Reveal();

        if (cell.value == 0)
            FloodReveal(cell);

        CheckProgress();
    }

    public void OnRightClick(MineCell cell)
    {
        if (cell.isRevealed) return;

        FlagInventory inventory = FlagInventory.Instance;
        if (inventory == null) return;

        if (!cell.isFlagged)
        {
            if (inventory.UseFlag())
                cell.ToggleFlag();
            else
                Debug.Log("[Minefield] Флагов нет!");
        }
        else
        {
            cell.ToggleFlag();
            inventory.ReturnFlag();
        }

        CheckProgress();
    }

    // -------------------------------------------------------
    // Проверка прогресса
    // -------------------------------------------------------

    void CheckProgress()
    {
        if (IsCleared) return;

        FlaggedDangerousMines = 0;  // отмеченные взрывоопасные флаги
        TotalFlaggedCells = 0; // отмеченные ВСЕ флаги
        bool allSafeRevealed = true;

        foreach (var cell in cells)
        {
            if (cell.isFlagged) TotalFlaggedCells++; // считаем ВСЕ флаги для худа

            if (cell.value == 9)
            {
                if (cell.isFlagged) FlaggedDangerousMines++; // считаем взрывоопасные
                else allSafeRevealed = false;
            }
            else
            {
                if (cell.isRevealed == false) allSafeRevealed = false;
            }
        }

        MinefieldHUD.Instance?.UpdateMineCounter(this);

        if (allSafeRevealed && FlaggedDangerousMines == TotalDangerousMines && TotalFlaggedCells == TotalDangerousMines)
        {
            IsCleared = true;
            Debug.Log($"[Minefield] '{gameObject.name}' очищено!");
            MinefieldHUD.Instance?.UpdateMineCounter(this);
            onFieldCleared.Invoke();
        }
    }

    // -------------------------------------------------------
    // Флудфилл
    // -------------------------------------------------------

    void FloodReveal(MineCell startCell)
    {
        Queue<MineCell> queue = new Queue<MineCell>();
        HashSet<MineCell> visited = new HashSet<MineCell>();

        queue.Enqueue(startCell);
        visited.Add(startCell);

        while (queue.Count > 0)
        {
            MineCell current = queue.Dequeue();
            foreach (var neighbor in GetNeighbors(current))
            {
                if (visited.Contains(neighbor)) continue;
                if (neighbor.isRevealed || neighbor.isFlagged) continue;
                if (neighbor.value == 9) continue;

                visited.Add(neighbor);
                neighbor.RevealSilent();

                if (neighbor.value == 0)
                    queue.Enqueue(neighbor);
            }
        }
    }

    public List<MineCell> GetAllCells() => cells;
}