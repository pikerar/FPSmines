using UnityEngine;
using UnityEngine.Events;

public class MinefieldGate : MonoBehaviour
{
    [Header("Поля которые нужно решить")]
    [SerializeField] private Minefield[] requiredFields;

    [Header("Событие когда все поля решены")]
    public UnityEvent onAllCleared;

    [Header("Событие когда условие снова не выполнено (опционально)")]
    public UnityEvent onConditionLost;

    private bool isOpen = false;

    void Update()
    {
        CheckCondition();
    }

    void CheckCondition()
    {
        if (requiredFields == null || requiredFields.Length == 0) return;

        bool allCleared = true;
        foreach (var field in requiredFields)
        {
            if (field == null) continue;
            if (!field.IsCleared) { allCleared = false; break; }
        }

        if (allCleared && !isOpen)
        {
            isOpen = true;
            Debug.Log($"[MinefieldGate] '{gameObject.name}' открыт!");
            onAllCleared.Invoke();
        }
        else if (!allCleared && isOpen)
        {
            // На случай если логика изменится и поле можно "разрешить"
            isOpen = false;
            onConditionLost.Invoke();
        }
    }

    // публичный геттер для других скриптов
    public bool IsOpen => isOpen;
}