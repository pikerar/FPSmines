using UnityEngine;
using UnityEngine.SceneManagement;

// ─────────────────────────────────────────────────────────────────────────────
//  MainMenu
//
//  Используется и на главном меню, и на экране проигрыша.
//
//  Главное меню:
//    Кнопка "Новая игра"  → NewGame()
//    Кнопка "Продолжить"  → Continue()   (активна только если есть чекпоинт)
//
//  Меню проигрыша:
//    Кнопка "В главное меню" → GoMain()
//    Кнопка "Заново"         → StartGame()   (перезапуск без чекпоинта)
//    Кнопка "С чекпоинта"    → LoadFromCheckpoint()
// ─────────────────────────────────────────────────────────────────────────────
public class MainMenu : MonoBehaviour
{
    [Header("Имя игровой сцены")]
    [SerializeField] private string gameSceneName = "BaseScene";

    [Header("Кнопка Продолжить (опционально)")]
    [Tooltip("Если назначена — будет авто-скрыта когда нет сохранения")]
    [SerializeField] private GameObject continueButton;

    void Start()
    {
        // Показываем/скрываем кнопку "Продолжить" в зависимости от наличия сейва
        //if (continueButton != null)
        //    continueButton.SetActive(CheckpointSystem.HasCheckpoint(gameSceneName));
    }

    // ── Главное меню ─────────────────────────────────────────────────────────

    // <summary>Новая игра — запустить сцену полностью с нуля.</summary>
    public void NewGame()
    {
        LevelManager.Instance.DeleteSave();
        SceneManager.LoadScene(gameSceneName);
    }

    // <summary>Продолжить — загрузить с последнего чекпоинта.</summary>
    public void Continue()
    {
        if (LevelManager.Instance.HasSave())
            SceneManager.LoadScene(LevelManager.Instance.LoadCheckpoint().sceneName);
        else
            NewGame();
    }

    // ── Меню проигрыша ───────────────────────────────────────────────────────

    // <summary>Перезапустить с нуля (старое StartGame).</summary>
    public void StartGame()
    {
        LevelManager.Instance.DeleteSave();
        SceneManager.LoadScene(gameSceneName);
    }

    // <summary>Загрузить с чекпоинта из меню проигрыша.</summary>
    public void LoadFromCheckpoint()
    {
        if (LevelManager.Instance.HasSave())
            SceneManager.LoadScene(LevelManager.Instance.LoadCheckpoint().sceneName);
        else
        {
            Debug.Log("[MainMenu] Чекпоинт не найден, запускаем заново.");
            LevelManager.Instance.DeleteSave();
            SceneManager.LoadScene(gameSceneName);
        }
    }

    /// <summary>Вернуться в главное меню.</summary>
    public void GoMain()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>Выйти из игры.</summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}