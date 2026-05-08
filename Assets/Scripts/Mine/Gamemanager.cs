using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Имя сцены экрана конца игры")]
    public string gameOverSceneName = "EndGame";

    [Header("Задержка перед переходом на EndGame (сек)")]
    public float gameOverDelay = 1.5f;

    [Header("Звук взрыва")]
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioSource explosionSource;
    [Range(0f, 1f)]
    [SerializeField] private float explosionVolume = 1f;

    [Header("Флеш взрыва")]
    [SerializeField] private Image explosionFlash;
    [SerializeField] private float flashDuration = 0.3f;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (explosionSource == null)
        {
            explosionSource = gameObject.AddComponent<AudioSource>();
            explosionSource.spatialBlend = 0f;
            explosionSource.playOnAwake = false;
        }

        if (explosionFlash != null)
        {
            var c = explosionFlash.color;
            c.a = 0f;
            explosionFlash.color = c;
        }
    }

    public void TriggerGameOver()
    {
        if (gameEnded) return;
        gameEnded = true;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        if (explosionClip != null)
            explosionSource.PlayOneShot(explosionClip, explosionVolume);

        if (explosionFlash != null)
            StartCoroutine(FlashRoutine());

        yield return new WaitForSeconds(gameOverDelay);

        LoadGameOverScene();
    }

    IEnumerator FlashRoutine()
    {
        SetFlashAlpha(1f);

        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            SetFlashAlpha(Mathf.Lerp(1f, 0f, t / flashDuration));
            yield return null;
        }

        SetFlashAlpha(0f);
    }

    void SetFlashAlpha(float alpha)
    {
        if (explosionFlash == null) return;
        var c = explosionFlash.color;
        c.a = alpha;
        explosionFlash.color = c;
    }

    void LoadGameOverScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void RestartGame()
    {
        gameEnded = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}