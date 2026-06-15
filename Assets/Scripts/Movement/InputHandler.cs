using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    // ƒвижение
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    // ѕрыжок
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }

    // ¬заимодействие
    public bool InteractPressed { get; private set; }

    // ћышь
    public bool LeftClickDown { get; private set; }
    public bool RightClickDown { get; private set; }

    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    // ѕауза Ч отдельно, потому что меню паузы должно работать всегда
    public bool PausePressed { get; private set; }

    // ”Ќ»¬≈–—јЋ№Ќјя проверка блокировки
    public bool IsInputBlocked => InputBlocker.IsBlocked;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        // ѕауза работает ¬—≈√ƒј, даже при блокировке
        PausePressed = Input.GetKeyDown(pauseKey);

        if (IsInputBlocked)
        {
            // —брасываем весь игровой инпут, но PausePressed уже записан
            Horizontal = 0f;
            Vertical = 0f;
            JumpPressed = false;
            JumpHeld = false;
            InteractPressed = false;
            LeftClickDown = false;
            RightClickDown = false;
            MouseX = 0f;
            MouseY = 0f;
            return;
        }

        // ќбычный инпут
        MouseX = Input.GetAxisRaw("Mouse X");
        MouseY = Input.GetAxisRaw("Mouse Y");

        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");

        JumpPressed = Input.GetKeyDown(jumpKey);
        JumpHeld = Input.GetKey(jumpKey);

        InteractPressed = Input.GetKeyDown(interactKey);

        LeftClickDown = Input.GetMouseButtonDown(0);
        RightClickDown = Input.GetMouseButtonDown(1);
    }
}