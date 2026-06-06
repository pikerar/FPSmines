using System.Collections;
using UnityEngine;
using TMPro;

public class SubtitleDisplay3D : MonoBehaviour
{
    [Header("Компоненты")]
    public TextMeshPro worldText;
    public CanvasGroup uiCanvas;       // опционально — UI субтитры внизу экрана
    public TextMeshProUGUI uiText;

    [Header("Настройки")]
    public float charsPerSecond = 20f;
    public float fadeTime = 0.25f;

    private Coroutine _current;

    // ── показать одну строку ──────────────────────────

    public void ShowLine(string line, float holdDuration)
    {
        if (_current != null) StopCoroutine(_current);
        _current = StartCoroutine(RunLine(line, holdDuration));
    }

    // Перегрузка — без hold, текст висит пока не вызовешь Hide()
    public void ShowLine(string line)
    {
        if (_current != null) StopCoroutine(_current);
        _current = StartCoroutine(RunLine(line, -1f));
    }

    public void Hide()
    {
        if (_current != null) StopCoroutine(_current);
        _current = StartCoroutine(Fade(GetAlpha(), 0f));
    }

    // ── цепочка строк ────────────────────────────────

    public void ShowSequence(string[] lines, float[] durations)
    {
        if (_current != null) StopCoroutine(_current);
        _current = StartCoroutine(RunSequence(lines, durations));
    }

    // ── корутины ─────────────────────────────────────

    IEnumerator RunSequence(string[] lines, float[] durations)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            float dur = i < durations.Length ? durations[i] : 3f;
            yield return RunLine(lines[i], dur);
        }
    }

    IEnumerator RunLine(string line, float holdDuration)
    {
        SetText(line, 0);
        yield return Fade(0f, 1f);

        // Typewriter
        int total = line.Length;
        float elapsed = 0f;
        while (elapsed < total / charsPerSecond)
        {
            elapsed += Time.deltaTime;
            int visible = Mathf.Clamp(Mathf.FloorToInt(elapsed * charsPerSecond), 0, total);
            SetVisible(visible);
            yield return null;
        }
        SetVisible(total);

        // Hold — если -1 висит до Hide()
        if (holdDuration >= 0f)
        {
            float typed = total / charsPerSecond;
            float hold = Mathf.Max(0f, holdDuration - typed - fadeTime);
            yield return new WaitForSeconds(hold);
            yield return Fade(1f, 0f);
        }
    }

    IEnumerator Fade(float from, float to)
    {
        float e = 0f;
        while (e < fadeTime)
        {
            e += Time.deltaTime;
            float a = Mathf.Lerp(from, to, e / fadeTime);
            ApplyAlpha(a);
            yield return null;
        }
        ApplyAlpha(to);
    }

    // ── хелперы ──────────────────────────────────────

    void SetText(string line, int visible)
    {
        if (worldText) { worldText.text = line; worldText.maxVisibleCharacters = visible; }
        if (uiText) { uiText.text = line; uiText.maxVisibleCharacters = visible; }
    }

    void SetVisible(int n)
    {
        if (worldText) worldText.maxVisibleCharacters = n;
        if (uiText) uiText.maxVisibleCharacters = n;
    }

    void ApplyAlpha(float a)
    {
        if (worldText) worldText.alpha = a;
        if (uiCanvas) uiCanvas.alpha = a;
    }

    float GetAlpha() => worldText ? worldText.alpha : 0f;
}