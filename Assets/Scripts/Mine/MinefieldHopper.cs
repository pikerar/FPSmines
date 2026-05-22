using UnityEngine;
using UnityEngine.Events;

public class MinefieldTrigger : MonoBehaviour
{
    public enum ConditionMode
    {
        AND,
        OR   
    }

    [Header("Минные поля")]
    [SerializeField] private Minefield[] requiredFields;

    [Header("Режим условия")]
    [SerializeField] private ConditionMode conditionMode = ConditionMode.AND;

    [Header("События")]
    public UnityEvent onConditionMet;   
    public UnityEvent onConditionLost; 

    [Header("Настройки")]
    [Tooltip("Если true — событие сработает только один раз")]
    [SerializeField] private bool fireOnce = true;

    private bool conditionMet = false;
    private bool fired = false;

    void Update()
    {
        if (fireOnce && fired) return;
        if (requiredFields == null || requiredFields.Length == 0) return;

        bool result = EvaluateCondition();

        if (result && !conditionMet)
        {
            conditionMet = true;
            fired = true;
            Debug.Log($"[MinefieldTrigger] '{gameObject.name}' — условие выполнено ({conditionMode})");
            onConditionMet.Invoke();
        }
        else if (!result && conditionMet)
        {
            conditionMet = false;
            onConditionLost.Invoke();
        }
    }

    bool EvaluateCondition()
    {
        switch (conditionMode)
        {
            case ConditionMode.AND:
                foreach (var field in requiredFields)
                {
                    if (field == null) continue;
                    if (!field.IsCleared) return false;
                }
                return true;

            case ConditionMode.OR:
                foreach (var field in requiredFields)
                {
                    if (field == null) continue;
                    if (field.IsCleared) return true;
                }
                return false;

            default:
                return false;
        }
    }

    public bool IsConditionMet => conditionMet;
}