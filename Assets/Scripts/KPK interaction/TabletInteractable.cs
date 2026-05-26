using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TabletInteractable : MonoBehaviour, IInteractable
{
    private TabletSpawner spawner;
    private string hintText;

    public void Setup(TabletSpawner spawnerRef, string hint)
    {
        spawner = spawnerRef;
        hintText = hint;

        // Убедимся что есть коллайдер для рейкаста
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = false;
    }

    public string GetInteractHint()
    {
        return hintText;
    }

    public void Interact()
    {
        spawner?.PickUpTablet();
    }

    // Для системы рейкаста (если используешь интерфейсы)
    public bool CanInteract()
    {
        return spawner != null;
    }
}