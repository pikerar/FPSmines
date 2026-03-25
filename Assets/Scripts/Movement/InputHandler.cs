using UnityEngine;

/// <summary>
/// Отдельный скрипт для обработки ввода.
/// Прикрепи на тот же GameObject что и PlayerMovement.
/// </summary>
public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    // Движение
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    // Прыжок
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }

    // Взаимодействие
    public bool InteractPressed { get; private set; }

    // Мышь
    public bool LeftClickDown { get; private set; }
    public bool RightClickDown { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");

        JumpPressed = Input.GetKey(jumpKey);
        JumpHeld = Input.GetKey(jumpKey);

        InteractPressed = Input.GetKeyDown(interactKey);

        LeftClickDown = Input.GetMouseButtonDown(0);
        RightClickDown = Input.GetMouseButtonDown(1);
    }
}