using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Глобальный менеджер игры
/// Накинуть на геймменеджер и усё
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Имя сцены экрана конца игры")]
    public string gameOverSceneName = "EndGame";

    [Header("Задержка перед переходом на EndGame (сек)")]
    public float gameOverDelay = 1.5f;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Вызывается MineCell при взрыве
    /// </summary>
    public void TriggerGameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        Invoke(nameof(LoadGameOverScene), gameOverDelay);
    }

    //void RevealAllMines()
    //{
    //    MineCell[] allCells = FindObjectsOfType<MineCell>();
    //    foreach (var cell in allCells)
    //    {
    //        if (cell.value == 9 && !cell.isRevealed)
    //        {
    //            cell.RevealSilent();
    //        }
    //    }
    //}

    /// <summary>
    /// Переход на экран эндгейма при проигрыше
    /// </summary>
    void LoadGameOverScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(gameOverSceneName);
    }

    /// <summary>
    /// Перезапуск уровня
    /// </summary>
    public void RestartGame()
    {
        gameEnded = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}