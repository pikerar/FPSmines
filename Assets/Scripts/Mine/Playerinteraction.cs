using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Дистанция луча (максимум видимости)")]
    [SerializeField] private float rayDistance = 10f;

    [Header("Дистанция взаимодействия (подсказка + клики)")]
    [SerializeField] private float interactDistance = 3f;

    [Header("Камера (если не назначена — ищет Camera.main)")]
    [SerializeField] private Camera playerCamera;

    private MineCell hoveredMine;
    private FlagBox hoveredBox;
    private BarrierButton hoveredButton;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        Debug.Log($"[Interaction] камера: {playerCamera?.gameObject.name}");
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    void HandleRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 0.1f);

        RaycastHit hit;

        MineCell newMine = null;
        FlagBox newBox = null;
        BarrierButton newButton = null;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {

            if (hit.distance <= interactDistance)
            {
                newMine = hit.collider.GetComponentInParent<MineCell>();
                Debug.Log($"[Raycast] MineCell найден: {newMine != null}");
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
            // Берём поле прямо из мины — работает с любым количеством полей на сцене
            Minefield field = hoveredMine.ParentField;
            if (field == null) return;

            if (input.LeftClickDown) field.OnLeftClick(hoveredMine);
            else if (input.RightClickDown) field.OnRightClick(hoveredMine);
        }
    }
}