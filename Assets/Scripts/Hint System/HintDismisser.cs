using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Закрывает WorldHint при выполнении одного из условий:
/// - Игрок зашёл в зону (DismissZone)
/// - Игрок подобрал предмет — вызвать Dismiss() из скрипта предмета
/// - Нажата кнопка (UI Button → Dismiss())
/// - Любое UnityEvent → Dismiss()
///
/// Один DismisserHint может обслуживать несколько WorldHint одновременно.
/// </summary>
public class HintDismisser : MonoBehaviour
{
    [Header("Подсказки для закрытия")]
    public WorldHintCompass[] hintsToHide;

    [Header("Тип закрытия")]
    public DismissType dismissType = DismissType.Zone;

    [Header("Зона (DismissType.Zone)")]
    [Tooltip("Тег объекта, вход которого закрывает подсказку")]
    public string playerTag = "Player";

    [Header("События")]
    public UnityEvent onHintDismissed;

    public enum DismissType
    {
        /// <summary>Вызвать Dismiss() из кода/кнопки/события</summary>
        Manual,
        /// <summary>Игрок заходит в триггер-зону этого объекта</summary>
        Zone,
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dismissType != DismissType.Zone) return;
        if (!other.CompareTag(playerTag)) return;
        Dismiss();
    }

    /// <summary>
    /// Вызвать для закрытия подсказки из любого места:
    /// кнопка UI, скрипт предмета, AnimationEvent и т.д.
    /// </summary>
    public void Dismiss()
    {
        foreach (var hint in hintsToHide)
        {
            if (hint != null) hint.Hide();
        }
        onHintDismissed?.Invoke();
    }
}