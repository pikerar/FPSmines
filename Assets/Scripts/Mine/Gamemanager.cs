using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Глобальный менеджер игры.
/// Помести на постоянный GameObject в сцене.
/// Добавь FlagInventory на тот же объект.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Имя сцены экрана конца игры")]
    public string gameOverSceneName = "GameOver";

    [Header("Задержка перед переходом на GameOver (сек)")]
    public float gameOverDelay = 1.5f;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Вызывается MineCell при взрыве
    /// </summary>
    public void TriggerGameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("[GameManager] GAME OVER!");

        // Показать все мины
        RevealAllMines();

        // Перейти на экран конца через задержку
        Invoke(nameof(LoadGameOverScene), gameOverDelay);
    }

    void RevealAllMines()
    {
        // Раскрываем все взрывоопасные мины для наглядности
        MineCell[] allCells = FindObjectsOfType<MineCell>();
        foreach (var cell in allCells)
        {
            if (cell.value == 9 && !cell.isRevealed)
            {
                cell.RevealSilent();
            }
        }
    }

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