using UnityEngine;
using TMPro;

/// <summary>
/// Прикрепляется к каждому префабу мины.
/// value: 0-8 = количество мин вокруг, 9 = сама мина
/// </summary>
public class MineCell : MonoBehaviour
{
    [Header("Значение ячейки (0-8 = число, 9 = мина)")]
    public int value = 0;

    [Header("UI на объекте и в небе")]
    public TextMeshPro screenText;   // текст на циферблате мины
    public TextMeshPro skyText;      // текст парящий над миной

    [Header("Объект флага на мине")]
    public GameObject flagObject;    // показывается при установке флага

    [Header("Состояние")]
    public bool isRevealed = false;
    public bool isFlagged = false;

    // Цвета цифр как в классическом сапёре
    private static readonly Color[] numberColors = new Color[]
    {
        Color.white,        // 0
        Color.blue,         // 1
        Color.green,        // 2
        Color.red,          // 3
        new Color(0f, 0f, 0.5f),    // 4 тёмно-синий
        new Color(0.5f, 0f, 0f),    // 5 тёмно-красный
        Color.cyan,         // 6
        Color.black,        // 7
        Color.gray          // 8
    };

    void Start()
    {
        // Скрываем тексты до взаимодействия
        if (screenText != null) screenText.text = "";
        if (skyText != null) skyText.text = "";
        if (flagObject != null) flagObject.SetActive(false);
    }

    /// <summary>
    /// Вызывается минным полем после расчёта матрицы
    /// </summary>
    public void SetValue(int val)
    {
        value = val;
    }

    /// <summary>
    /// ЛКМ — открыть ячейку
    /// </summary>
    public void Reveal()
    {
        if (isRevealed || isFlagged) return;

        isRevealed = true;

        if (value == 9)
        {
            // Взрыв!
            Explode();
            return;
        }

        // Показываем число (0 показываем как пустоту или тоже отображаем)
        string display = value == 0 ? "" : value.ToString();
        Color col = value > 0 && value <= 8 ? numberColors[value] : Color.white;

        if (screenText != null)
        {
            screenText.text = display;
            screenText.color = col;
        }
        if (skyText != null)
        {
            skyText.text = display;
            skyText.color = col;
        }
    }

    /// <summary>
    /// ПКМ — поставить/снять флаг
    /// </summary>
    /// <returns>true если флаг поставлен, false если снят</returns>
    public bool ToggleFlag()
    {
        if (isRevealed) return isFlagged;

        isFlagged = !isFlagged;

        if (flagObject != null)
            flagObject.SetActive(isFlagged);

        return isFlagged;
    }

    private void Explode()
    {
        Debug.Log("ВЗРЫВ! Переход на экран конца игры.");
        // Небольшая задержка для эффектов если нужно
        GameManager.Instance?.TriggerGameOver();
    }

    /// <summary>
    /// Открыть без анимации (для авто-раскрытия нулей)
    /// </summary>
    public void RevealSilent()
    {
        if (isRevealed || isFlagged) return;
        isRevealed = true;

        string display = value == 0 ? "" : value.ToString();
        Color col = value > 0 && value <= 8 ? numberColors[value] : Color.white;

        if (screenText != null) { screenText.text = display; screenText.color = col; }
        if (skyText != null) { skyText.text = display; skyText.color = col; }
    }
}