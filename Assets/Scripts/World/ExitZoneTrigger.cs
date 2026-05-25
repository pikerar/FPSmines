using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ExitZoneTrigger : MonoBehaviour
{
    [Header("Условие выхода")]
    [SerializeField] private MinefieldTrigger minefieldTrigger;

    [Header("Сцена для загрузки")]
    [SerializeField] private string endSceneName = "EndGame";

    [Header("Таймер")]
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private TMP_Text countdownLabel;     // UI-текст таймера
    [SerializeField] private GameObject countdownPanel;   // панель (можно null)

    private bool isCountingDown = false;
    private Coroutine countdownCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!minefieldTrigger.IsConditionMet) return;


        if (!isCountingDown)
            countdownCoroutine = StartCoroutine(Countdown());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        // игрок вышел из зоны — сбрасываем таймер
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        isCountingDown = false;
        SetPanelVisible(false);
    }

    private IEnumerator Countdown()
    {
        isCountingDown = true;
        SetPanelVisible(true);

        float remaining = countdownDuration;

        while (remaining > 0f)
        {
            if (countdownLabel != null)
                countdownLabel.text = $"Выход через: {Mathf.CeilToInt(remaining)}";

            remaining -= Time.deltaTime;
            yield return null;
        }

        if (countdownLabel != null)
            countdownLabel.text = "Загрузка...";

        SceneManager.LoadScene(endSceneName);
    }

    private void SetPanelVisible(bool visible)
    {
        if (countdownPanel != null)
            countdownPanel.SetActive(visible);
    }
}