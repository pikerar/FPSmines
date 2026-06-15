using UnityEngine;

public class SubtitleDistanceScaler : MonoBehaviour
{
    [Header("Настройки масштаба")]
    [Tooltip("Базовое расстояние, при котором масштаб = 1")]
    [SerializeField] private float baseDistance = 3f;

    [Tooltip("Минимальный масштаб текста")]
    [SerializeField] private float minScale = 0.5f;

    [Tooltip("Максимальный масштаб текста")]
    [SerializeField] private float maxScale = 2f;

    [Header("Ссылка на камеру (оставь пустым для Camera.main)")]
    [SerializeField] private Camera targetCamera;

    private Transform camTransform;
    private Vector3 originalScale;

    void Start()
    {
        // Запоминаем изначальный масштаб
        originalScale = transform.localScale;

        // Берём камеру
        if (targetCamera == null)
            targetCamera = Camera.main;

        camTransform = targetCamera.transform;
    }

    void Update()
    {
        if (camTransform == null) return;

        // Считаем расстояние до камеры
        float distance = Vector3.Distance(transform.position, camTransform.position);

        // Считаем множитель масштаба: чем дальше, тем больше текст
        // При baseDistance множитель = 1, при 2x расстоянии = 2x масштаб
        float scaleMultiplier = distance / baseDistance;

        // Ограничиваем min/max
        scaleMultiplier = Mathf.Clamp(scaleMultiplier, minScale, maxScale);

        // Применяем масштаб
        transform.localScale = originalScale * scaleMultiplier;
    }
}