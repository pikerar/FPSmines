using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Минное поле — считывает все дочерние мины (объеты внутри родительского объекта с этим скриптом),
/// строит матрицу, расставляет значения и обрабатывает клики игрока.
/// кароче можно сделать вручную расстановку мин, а можно фулл генерируемую
/// </summary>
public class Minefield : MonoBehaviour
{
    [Header("Настройка поля")]
    public bool randomizeMines = false;
    public int mineCount = 10; //кол-во взрывоопасных мин 

    [Header("Радиус соседства (единицы Unity)")]
    public float neighborRadius = 3.1f;

    private List<MineCell> cells = new List<MineCell>();

    void Awake()
    {
        CollectCells();
        if (randomizeMines) PlaceRandomMines();
        CalculateValues();
    }

    void CollectCells()
    {
        cells.Clear();
        MineCell[] found = GetComponentsInChildren<MineCell>();
        cells.AddRange(found);
        Debug.Log($"[Minefield] найдено мин: {cells.Count}");
    }

    void PlaceRandomMines()
    {
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
    }

    void CalculateValues()
    {
        foreach (var cell in cells)
        {
            if (cell.value == 9) continue;

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

    /// <summary>
    /// ЛКМ по мине
    /// </summary>
    public void OnLeftClick(MineCell cell)
    {
        if (cell.isRevealed || cell.isFlagged) return;

        cell.Reveal();

        if (cell.value == 0 && !cell.isFlagged)
        {
            FloodReveal(cell);
        }
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
            if (inventory.UseFlag())
            {
                cell.ToggleFlag();
            }
            else
            {
                // вывод уведомления что нет флагов   
            }
        }
        else
        {
            cell.ToggleFlag();
            inventory.ReturnFlag();
        }
    }

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
    //дебаг
    public List<MineCell> GetAllCells() => cells;
}