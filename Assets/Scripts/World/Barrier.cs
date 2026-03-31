using UnityEngine;
using DG.Tweening;

public class Barrier : MonoBehaviour
{
    [Header("Углы поворота")]
    [SerializeField] private float closedAngle = 0f;
    [SerializeField] private float openAngle = 80f;

    [Header("Ось вращения")]
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    [Header("Анимация")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;

    private bool isOpen = false;
    private Tween currentTween;

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        AnimateTo(openAngle);
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        AnimateTo(closedAngle);
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    void AnimateTo(float angle)
    {
        currentTween?.Kill();

        Vector3 targetRotation = rotationAxis * angle;
        currentTween = transform.DOLocalRotate(targetRotation, duration)
                                .SetEase(easeType);
    }
}