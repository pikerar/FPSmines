using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Минное поле — считывает все дочерние MineCell,
/// строит матрицу, расставляет значения и обрабатывает клики игрока.
/// 
/// Настройка в редакторе:
/// 1. Создай пустой GameObject "Minefield"
/// 2. Расставь префабы мин внутри него как дочерние объекты
/// 3. Помечай мины которые должны быть взрывоопасными через Inspector (isMine = true)
///    ИЛИ задай mineCount и скрипт расставит мины случайно
/// </summary>
public class Minefield : MonoBehaviour
{
    [Header("Настройка поля")]
    [Tooltip("Если true — мины расставляются случайно по mineCount. " +
             "Если false — читает компонент MineCell.value == 9 из сцены.")]
    public bool randomizeMines = false;
    public int mineCount = 10;

    [Header("Радиус соседства (единицы Unity)")]
    [Tooltip("Две ячейки считаются соседями если расстояние между ними меньше этого значения")]
    public float neighborRadius = 1.6f;

    // Все ячейки поля
    private List<MineCell> cells = new List<MineCell>();

    void Awake()
    {
        CollectCells();
        if (randomizeMines) PlaceRandomMines();
        CalculateValues();
    }

    // -------------------------------------------------------
    // Инициализация
    // -------------------------------------------------------

    void CollectCells()
    {
        cells.Clear();
        // Берём все MineCell среди дочерних (включая вложенные)
        MineCell[] found = GetComponentsInChildren<MineCell>();
        cells.AddRange(found);
        Debug.Log($"[Minefield] Найдено ячеек: {cells.Count}");
    }

    void PlaceRandomMines()
    {
        // Сбрасываем всё
        foreach (var c in cells) c.SetValue(0);

        int placed = 0;
        int attempts = 0;
        int maxAttempts = cells.Count * 10;

        while (placed < mineCount && attempts < maxAttempts)
        {
            attempts++;
            int idx = Random.Range(0, cells.Count);
            if (cells[idx].value != 9)
            {
                cells[idx].SetValue(9);
                placed++;
            }
        }
        Debug.Log($"[Minefield] Расставлено мин: {placed}");
    }

    void CalculateValues()
    {
        foreach (var cell in cells)
        {
            if (cell.value == 9) continue; // уже мина

            int count = 0;
            foreach (var neighbor in GetNeighbors(cell))
            {
                if (neighbor.value == 9) count++;
            }
            cell.SetValue(count);
        }
    }

    List<MineCell> GetNeighbors(MineCell cell)
    {
        List<MineCell> result = new List<MineCell>();
        foreach (var other in cells)
        {
            if (other == cell) continue;
            float dist = Vector3.Distance(cell.transform.position, other.transform.position);
            if (dist <= neighborRadius) result.Add(other);
        }
        return result;
    }

    // -------------------------------------------------------
    // Взаимодействие (вызывается из PlayerInteraction)
    // -------------------------------------------------------

    /// <summary>
    /// ЛКМ по мине
    /// </summary>
    public void OnLeftClick(MineCell cell)
    {
        if (cell.isRevealed) return;

        // Если на мине флаг — ЛКМ его снимает
        if (cell.isFlagged)
        {
            cell.ToggleFlag();
            FlagInventory.Instance?.ReturnFlag();
            return;
        }

        cell.Reveal();

        if (cell.value == 0)
            FloodReveal(cell);
    }

    /// <summary>
    /// ПКМ по мине
    /// </summary>
    public void OnRightClick(MineCell cell)
    {
        if (cell.isRevealed) return;

        FlagInventory inventory = FlagInventory.Instance;
        if (inventory == null) return;

        if (!cell.isFlagged)
        {
            // Пытаемся поставить флаг
            if (inventory.UseFlag())
            {
                cell.ToggleFlag();
            }
            else
            {
                Debug.Log("[Minefield] Флагов нет! Иди к ящику.");
                // Можно показать UI-подсказку
            }
        }
        else
        {
            // Снимаем флаг — возвращаем в инвентарь
            cell.ToggleFlag();
            inventory.ReturnFlag();
        }
    }

    // -------------------------------------------------------
    // Флудфилл для нулевых ячеек
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

    // -------------------------------------------------------
    // Публичный доступ к ячейкам (для отладки)
    // -------------------------------------------------------
    public List<MineCell> GetAllCells() => cells;
}