using UnityEngine;
using TMPro;

/// <summary>
/// Как я горд тем, что я додумался до этого))))0
/// </summary>
public class MineCell : MonoBehaviour
{
    [Header("Значение ячейки (0-8 = число, 9 = мина)")]
    public int value = 0;

    [Header("UI на объекте и в небе")]
    public TextMeshPro screenText;   // текст на циферблате мины
    public TextMeshPro skyText;      // текст парящий над миной

    [Header("Объект флага на мине")]
    public GameObject flagObject;

    [Header("Состояние")]
    public bool isRevealed = false;
    public bool isFlagged = false;

    private static readonly Color[] numberColors = new Color[]
    {
        Color.white,        // 0 белый
        Color.blue,         // 1 синий
        Color.green,        // 2 зеленый
        Color.red,          // 3 красный
        new Color(0f, 0f, 0.5f),    // 4 тёмно-синий
        new Color(0.5f, 0f, 0f),    // 5 тёмно-красный
        Color.cyan,         // 6 голубой
        Color.black,        // 7 черный
        Color.gray          // 8 соболезную...
    };

    void Start() // на старте все 3 окна отсутствуют, при взаимодействии они отображаются
    {
        if (screenText != null) screenText.text = "";
        if (skyText != null) skyText.text = "";
        if (flagObject != null) flagObject.SetActive(false);
    }

    /// <summary>
    /// Вызывается минным полем после расчёта матрицы
    /// Т.е. перекид данных с минного поля в каждую отдельную мину
    /// </summary>
    public void SetValue(int val)
    {
        value = val;
    }

    /// <summary>
    /// Взаимодействие с миной на ЛКМ
    /// </summary>
    public void Reveal()
    {
        if (isRevealed || isFlagged) return;

        isRevealed = true;

        if (value == 9)
        {
            // анлак
            Explode();
            return;
        }

        string display = value.ToString();
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
    /// Взаимодействие с миной на ПКМ
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
        // сюда же добавить звук, и мб визуал
        GameManager.Instance?.TriggerGameOver(); // или во внутрь
        // или сюда
    }

    /// <summary>
    /// Авто открытие нулей
    /// </summary>
    public void RevealSilent()
    {
        if (isRevealed || isFlagged) return;
        isRevealed = true;

        string display = value.ToString();
        Color col = value > 0 && value <= 8 ? numberColors[value] : Color.white;

        if (screenText != null) { screenText.text = display; screenText.color = col; }
        if (skyText != null) { skyText.text = display; skyText.color = col; }
    }
}