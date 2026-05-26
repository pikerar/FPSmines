using UnityEngine;

public class TabletDebug : MonoBehaviour
{
    void OnMouseOver()
    {
        Debug.Log($"[TabletDebug] Мышь над {gameObject.name}");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[TabletDebug] Триггер: {other.name} вошёл в {gameObject.name}");
    }
}