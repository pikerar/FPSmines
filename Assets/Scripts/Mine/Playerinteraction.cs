using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Дистанция луча (максимум видимости)")]
    [SerializeField] private float rayDistance = 10f;

    [Header("Дистанция взаимодействия (подсказка + клики)")]
    [SerializeField] private float interactDistance = 3f;

    [Header("Камера (если не назначена — ищет Camera.main)")]
    [SerializeField] private Camera playerCamera;

    private Minefield minefield;
    private MineCell hoveredMine;
    private FlagBox hoveredBox;
    private BarrierButton hoveredButton;

    private Collider OldCollider = null;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        minefield = FindFirstObjectByType<Minefield>();
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    void HandleRaycast()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        MineCell newMine = null;
        FlagBox newBox = null;
        BarrierButton newButton = null;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider == OldCollider)
            {
                return;
            }
            OldCollider = hit.collider;
            if (hit.distance <= interactDistance)
            {
                newMine = hit.collider.GetComponentInParent<MineCell>();
                if (newMine == null) newBox = hit.collider.GetComponentInParent<FlagBox>();
                if (newMine == null && newBox == null) newButton = hit.collider.GetComponentInParent<BarrierButton>();
            }
                        
        }

        hoveredMine = newMine;
        hoveredBox = newBox;
        hoveredButton = newButton;

        MinefieldHUD.Instance?.UpdateHoverHint(hoveredMine, hoveredBox, hoveredButton);
    }

    void HandleInput()
    {
        var input = InputHandler.Instance;
        if (input == null) return;

        if (hoveredMine != null)
        {
            if (input.LeftClickDown) minefield?.OnLeftClick(hoveredMine);
            else if (input.RightClickDown) minefield?.OnRightClick(hoveredMine);
        }
    }
}