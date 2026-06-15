using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [SerializeField] private GameObject pauseRoot;

    private bool isPaused;
    public bool IsPaused => isPaused;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (InputHandler.Instance != null && InputHandler.Instance.PausePressed)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseRoot.SetActive(true);
        InputBlocker.Block("PauseMenu");
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void Resume()
    {
        pauseRoot.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        InputBlocker.Unblock("PauseMenu");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }

    public void ExitToMainMenu()
    {
        InputBlocker.Clear();
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }

    // ── Новые методы для кнопок ───────────────────────────────────────────────

    // <summary>
    // Кнопка "Начать заново" — перезапустить локацию с нуля, без чекпоинта.
    // </summary>
    public void RestartFresh()
    {
        InputBlocker.Clear();
        LevelManager.Instance.DeleteSave();
        SceneStateManager.Instance.ClearState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Resume();
    }

    // <summary>
    // Кнопка "Загрузить чекпоинт" — перезапустить с последней точки сохранения.
    // Если чекпоинта нет — запускает сцену заново(как RestartFresh).
    // </summary>
    public void RestartFromCheckpoint()
    {
        InputBlocker.Clear();
        string sceneName = SceneManager.GetActiveScene().name;

        if (LevelManager.Instance.HasSave())
        {
            SceneManager.LoadScene(sceneName);
            Resume(); }
            
            
        else
        {
            LevelManager.Instance.DeleteSave();
            SceneManager.LoadScene(sceneName);
            Resume();
        }
    }
}