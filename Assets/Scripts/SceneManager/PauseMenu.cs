using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [SerializeField] private GameObject pauseRoot;
    [SerializeField] private NewPlayerMovement playerMovement;
    [SerializeField] private CameraLook cameraLook;

    [SerializeField] private PlayerInteraction playerInteraction;

    private bool isPaused;
    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseRoot.SetActive(true);

        if (playerMovement != null)
            playerMovement.InputLocked = true;

        if (cameraLook != null)
            cameraLook.InputLocked = true;

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

        if (playerMovement != null)
            playerMovement.InputLocked = false;

        if (cameraLook != null)
            cameraLook.InputLocked = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}