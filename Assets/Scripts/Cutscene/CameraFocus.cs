using System.Collections;
using UnityEngine;

public enum FocusEndMode { ByTime, ByStop }

public class CameraFocus : MonoBehaviour
{
    [Header("Ссылки — под твою структуру")]
    public Transform playerRoot;          // GameObject с CameraLook (крутится по Y)
    public Transform camHolder;           // крутится по X (вертикаль)
    public CameraLook cameraLook;         // чтобы восстановить rotX после фокуса
    public NewPlayerMovement playerMovement;

    [Header("Режим завершения")]
    public FocusEndMode endMode = FocusEndMode.ByTime;
    public float focusDuration = 5f;

    [Header("Скорость")]
    public float lookSpeed = 1.0f;
    public float returnSpeed = 0.7f;

    [Header("Обзор во время фокуса")]
    public float maxLookOffset = 30f;
    public float headReturnSpeed = 3f;

    public System.Action OnFocusStarted;
    public System.Action OnFocusEnded;

    // сохранённые углы до фокуса
    private float _savedRootY;       // горизонталь playerRoot
    private float _savedCamX;        // вертикаль camHolder (= rotX в CameraLook)

    // целевые углы фокуса
    private float _targetRootY;
    private float _targetCamX;

    // текущее отклонение головой во время фокуса
    private float _offsetY = 0f;
    private float _offsetX = 0f;

    private bool _focused = false;
    private Coroutine _focusCoroutine;

    // ── публичные методы ─────────────────────────────

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

    // ── корутины ─────────────────────────────────────

    IEnumerator RunFocus(Transform target)
    {
        _focused = true;

        // Блокируем ввод через твои скрипты
        cameraLook.InputLocked = true;
        playerMovement.InputLocked = true;

        // Запоминаем текущие углы
        _savedRootY = playerRoot.eulerAngles.y;
        _savedCamX = cameraLook.rotX;   // нужно сделать rotX публичным — см. ниже

        // Считаем нужные углы чтобы смотреть на target
        Vector3 dir = target.position - camHolder.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        _targetRootY = lookRot.eulerAngles.y;
        // конвертируем pitch в диапазон [-180, 180]
        float pitch = lookRot.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
        _targetCamX = pitch;   // CameraLook хранит rotX инвертированным

        _offsetY = 0f;
        _offsetX = 0f;

        // Плавный поворот к цели
        yield return SmoothLook(_savedRootY, _savedCamX, _targetRootY, _targetCamX, lookSpeed);

        OnFocusStarted?.Invoke();

        if (endMode == FocusEndMode.ByTime)
        {
            yield return new WaitForSeconds(focusDuration);
            yield return EndFocus();
        }
        // ByStop — ждём StopFocus() снаружи
    }

    IEnumerator EndFocus()
    {
        // Возвращаемся к сохранённым углам
        float fromY = playerRoot.eulerAngles.y;
        float fromX = cameraLook.rotX;

        yield return SmoothLook(fromY, fromX, _savedRootY, _savedCamX, returnSpeed);

        // Восстанавливаем rotX в CameraLook чтобы не было рывка
        cameraLook.rotX = _savedCamX;
        cameraLook.InputLocked = false;
        playerMovement.InputLocked = false;

        _focused = false;
        OnFocusEnded?.Invoke();
    }

    IEnumerator SmoothLook(float fromY, float fromX, float toY, float toX, float duration)
    {
        float e = 0f;
        while (e < duration)
        {
            e += Time.deltaTime;
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
        // Горизонталь — крутим весь playerRoot
        Vector3 re = playerRoot.eulerAngles;
        playerRoot.eulerAngles = new Vector3(re.x, rootY, re.z);

        // Вертикаль — крутим camHolder
        camHolder.localRotation = Quaternion.Euler(camX, 0f, 0f);

        // Синхронизируем rotX в CameraLook
        cameraLook.rotX = camX;
    }

    // ── обзор головой во время фокуса ────────────────

    void Update()
    {
        if (!_focused) return;

        float mx = Input.GetAxisRaw("Mouse X") * 2f;
        float my = Input.GetAxisRaw("Mouse Y") * 2f;

        _offsetY += mx;
        _offsetX -= my;

        _offsetY = Mathf.Clamp(_offsetY, -maxLookOffset, maxLookOffset);
        _offsetX = Mathf.Clamp(_offsetX, -maxLookOffset * 0.5f, maxLookOffset * 0.5f);

        bool moving = Mathf.Abs(mx) > 0.01f || Mathf.Abs(my) > 0.01f;

        if (!moving)
        {
            _offsetY = Mathf.MoveTowards(_offsetY, 0f, Time.deltaTime * headReturnSpeed * 20f);
            _offsetX = Mathf.MoveTowards(_offsetX, 0f, Time.deltaTime * headReturnSpeed * 20f);
        }

        ApplyAngles(_targetRootY + _offsetY, _targetCamX + _offsetX);
    }
}