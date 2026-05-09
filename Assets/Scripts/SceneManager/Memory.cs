using UnityEngine;
using TMPro;

public class Memorys : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;

    [TextArea(2, 5)]
    [SerializeField] private string[] memories;

    private void Awake()
    {
        ShowRandomMemory();
    }

    public void ShowRandomMemory()
    {
        if (targetText == null) return;
        if (memories == null || memories.Length == 0) return;

        int index = Random.Range(0, memories.Length);
        targetText.text = memories[index];
    }
}