using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public Transform buttonTop;      // Подвижная часть кнопки
    public float pressDepth = 0.1f;  // Насколько сильно она утапливается
    public float pressSpeed = 3f;    // Скорость смещения
    public bool isPressed = false;

    private Vector3 startPos;
    private Vector3 pressedPos;
    private int objectsOnButton = 0; // Счётчик, чтобы кнопка поднималась, когда игрок ушёл

    void Start()
    {
        if (buttonTop == null)
            buttonTop = transform; // Если не задано — работать с самим объектом

        startPos = buttonTop.localPosition;
        pressedPos = startPos - new Vector3(0, pressDepth, 0);
    }

    void Update()
    {
        Vector3 targetPos = isPressed ? pressedPos : startPos;
        buttonTop.localPosition = Vector3.Lerp(
            buttonTop.localPosition,
            targetPos,
            Time.deltaTime * pressSpeed
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectsOnButton++;
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectsOnButton--;
            if (objectsOnButton <= 0) isPressed = false;
        }
    }
}
