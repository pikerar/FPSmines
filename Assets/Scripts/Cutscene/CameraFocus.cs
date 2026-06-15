using System.Collections;
using UnityEngine;

public enum FocusEndMode { ByTime, ByStop }

public class CameraFocus : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform playerRoot;
    public Transform camHolder;
    public CameraLook cameraLook;
    public NewPlayerMovement playerMovement;

    [Header("Настройки")]
    public FocusEndMode endMode = FocusEndMode.ByTime;
    public float focusDuration = 5f;
    public float lookSpeed = 1.0f;
    public float returnSpeed = 0.7f;
    public float maxLookOffset = 30f;
    public float headReturnSpeed = 3f;

    public System.Action OnFocusStarted;
    public System.Action OnFocusEnded;

    public bool IsFocused => _focused;

    private float _savedRootY;
    private float _savedCamX;
    private float _targetRootY;
    private float _targetCamX;
    private float _offsetY;
    private float _offsetX;
    private bool _focused;
    private Coroutine _focusCoroutine;

    public void StartFocus(Transform target)
    {
        if (_focused) return;
        _focusCoroutine = StartCoroutine(RunFocus(target));
    }

    public void StopFocus()
    {
        if (!_focused) return;
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
        StartCoroutine(EndFocus());
    }

    IEnumerator RunFocus(Transform target)
    {
        _focused = true;

        // БЛОКИРУЕМ ЧЕРЕЗ МЕНЕДЖЕР
        InputBlocker.Block("CameraFocus");

        // Запоминаем углы
        _savedRootY = playerRoot.eulerAngles.y;
        _savedCamX = cameraLook.rotX;

        // Считаем целевые углы
        Vector3 dir = target.position - camHolder.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        _targetRootY = lookRot.eulerAngles.y;
        float pitch = lookRot.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
        _targetCamX = pitch;

        _offsetY = 0f;
        _offsetX = 0f;

        yield return SmoothLook(_savedRootY, _savedCamX, _targetRootY, _targetCamX, lookSpeed);

        OnFocusStarted?.Invoke();

        if (endMode == FocusEndMode.ByTime)
        {
            yield return new WaitForSeconds(focusDuration);
            yield return EndFocus();
        }
    }

    IEnumerator EndFocus()
    {
        float fromY = playerRoot.eulerAngles.y;
        float fromX = cameraLook.rotX;

        yield return SmoothLook(fromY, fromX, _savedRootY, _savedCamX, returnSpeed);

        cameraLook.rotX = _savedCamX;

        // РАЗБЛОКИРУЕМ ЧЕРЕЗ МЕНЕДЖЕР
        InputBlocker.Unblock("CameraFocus");

        _focused = false;
        OnFocusEnded?.Invoke();
    }

    IEnumerator SmoothLook(float fromY, float fromX, float toY, float toX, float duration)
    {
        float e = 0f;
        while (e < duration)
        {
            // Time.timeScale = 0 во время паузы — корутины НЕ работают
            // Но если пауза закроется, продолжим с того же места
            e += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, e / duration);

            float y = Mathf.LerpAngle(fromY, toY, t);
            float x = Mathf.Lerp(fromX, toX, t);

            ApplyAngles(y, x);
            yield return null;
        }
        ApplyAngles(toY, toX);
    }

    void ApplyAngles(float rootY, float camX)
    {
        Vector3 re = playerRoot.eulerAngles;
        playerRoot.eulerAngles = new Vector3(re.x, rootY, re.z);
        camHolder.localRotation = Quaternion.Euler(camX, 0f, 0f);
        cameraLook.rotX = camX;
    }

    void Update()
    {
        if (!_focused) return;

        // Осмотр головой только если фокус — единственная блокировка инпута
        if (InputBlocker.IsBlocked && !InputBlocker.IsBlockedBy("CameraFocus"))
            return;

        float mx = Input.GetAxisRaw("Mouse X") * 2f;
        float my = Input.GetAxisRaw("Mouse Y") * 2f;

        _offsetY += mx;
        _offsetX -= my;

        _offsetY = Mathf.Clamp(_offsetY, -maxLookOffset, maxLookOffset);
        _offsetX = Mathf.Clamp(_offsetX, -maxLookOffset * 0.5f, maxLookOffset * 0.5f);

        bool moving = Mathf.Abs(mx) > 0.01f || Mathf.Abs(my) > 0.01f;

        if (!moving)
        {
            _offsetY = Mathf.MoveTowards(_offsetY, 0f, Time.unscaledDeltaTime * headReturnSpeed * 20f);
            _offsetX = Mathf.MoveTowards(_offsetX, 0f, Time.unscaledDeltaTime * headReturnSpeed * 20f);
        }

        ApplyAngles(_targetRootY + _offsetY, _targetCamX + _offsetX);
    }
}