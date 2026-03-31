using UnityEngine;
using System.Collections;

public class BarrierButton : MonoBehaviour
{
    [Header("Шлагбаум")]
    [SerializeField] private Barrier barrier;

    [Header("Задержка перед подъёмом (сек)")]
    [SerializeField] private float delayBeforeOpen = 3f;

    [Header("Дистанция взаимодействия")]
    [SerializeField] private float interactDistance = 2f;

    public bool IsActivated { get; private set; } = false;

    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (IsActivated) return;
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        bool nearby = dist <= interactDistance;

        if (nearby && InputHandler.Instance != null && InputHandler.Instance.InteractPressed)
            Activate();
    }

    void Activate()
    {
        IsActivated = true;
        StartCoroutine(OpenSequence());
    }

    IEnumerator OpenSequence()
    {
        // тут будет бубнеж

        yield return new WaitForSeconds(delayBeforeOpen);

        barrier?.Open();
    }
}