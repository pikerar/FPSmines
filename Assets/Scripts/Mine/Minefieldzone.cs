using UnityEngine;

[RequireComponent(typeof(Minefield))]
public class MinefieldZone : MonoBehaviour
{
    private Minefield minefield;

    void Awake()
    {
        minefield = GetComponent<Minefield>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        MinefieldHUD.Instance?.ShowMineCounter(minefield);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        MinefieldHUD.Instance?.HideMineCounter();
    }
}