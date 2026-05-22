using UnityEngine;
using TMPro;


public class MineCell : MonoBehaviour
{
    [Header("Значение ячейки (0-8 = число, 9 = мина)")]
    public int value = 0;

    [Header("UI на объекте и в небе")]
    public TextMeshPro screenText; 
    public TextMeshPro skyText;     

    [Header("Объект флага на мине")]
    public GameObject flagObject;  

    [Header("Состояние")]
    public bool isRevealed = false;
    public bool isFlagged = false;

    public Minefield ParentField { get; set; }

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
        if (screenText != null) screenText.text = "";
        if (skyText != null) skyText.text = "";
        if (flagObject != null) flagObject.SetActive(false);
    }

    public void SetValue(int val)
    {
        value = val;
        Debug.Log($"[MineCell] {gameObject.name} получил value={val}");
    }

    public void Reveal()
    {
        Debug.Log($"[MineCell] Reveal на {gameObject.name}: value={value}, isRevealed={isRevealed}, isFlagged={isFlagged}");

        if (isRevealed || isFlagged) return;

        isRevealed = true;

        if (value == 9)
        {
            Debug.Log($"[MineCell] ВЗРЫВ на {gameObject.name}!");
            Explode();
            return;
        }

        string display = value.ToString();
        Color col = value <= 8 ? numberColors[value] : Color.white;

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
        GameManager.Instance?.TriggerGameOver();
    }

    public void RevealSilent()
    {
        if (isRevealed || isFlagged) return;
        isRevealed = true;

        string display = value.ToString();
        Color col = value <= 8 ? numberColors[value] : Color.white;

        if (screenText != null) { screenText.text = display; screenText.color = col; }
        if (skyText != null) { skyText.text = display; skyText.color = col; }
    }
}