using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Дистанция луча (максимум видимости)")]
    [SerializeField] private float rayDistance;

    [Header("Дистанция взаимодействия (подсказка + клики)")]
    [SerializeField] private float interactDistance;

    [Header("Камера (если не назначена — ищет Camera.main)")]
    [SerializeField] private Camera playerCamera;

    private InteractionSoundPlayer soundPlayer;
    private MineCell hoveredMine;
    private FlagBox hoveredBox;
    private InteractableButton hoveredButton;  
    private Minefield subscribedField;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        soundPlayer = Object.FindFirstObjectByType<InteractionSoundPlayer>();
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

        MineCell newMine = null;
        FlagBox newBox = null;
        InteractableButton newButton = null;  

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.distance <= interactDistance)
            {
                newMine = hit.collider.GetComponentInParent<MineCell>();
                Debug.Log($"[Raycast] MineCell найден: {newMine != null}");
                if (newMine == null) newBox = hit.collider.GetComponentInParent<FlagBox>();
                if (newMine == null && newBox == null) newButton = hit.collider.GetComponentInParent<InteractableButton>();
            }
        }

        if (hoveredBox != newBox)
        {
            if (hoveredBox != null) hoveredBox.OnRefilled -= OnBoxRefilled;
            if (newBox != null) newBox.OnRefilled += OnBoxRefilled;
        }

        Minefield newField = newMine?.ParentField;
        if (subscribedField != newField)
        {
            if (subscribedField != null)
            {
                subscribedField.OnFlagPlaced -= OnFlagPlaced;
                subscribedField.OnFlagRemoved -= OnFlagRemoved;
            }
            if (newField != null)
            {
                newField.OnFlagPlaced += OnFlagPlaced;
                newField.OnFlagRemoved += OnFlagRemoved;
            }
            subscribedField = newField;
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

        if (hoveredButton != null && input.InteractPressed)
        {
            hoveredButton.Activate();
            return;
        }

        // Взаимодействие с минным полем
        if (hoveredMine != null)
        {
            Minefield field = hoveredMine.ParentField;
            if (field == null) return;

            if (input.LeftClickDown)
            {
                field.OnLeftClick(hoveredMine);
                soundPlayer?.Play("mine_left_click");
            }
            else if (input.RightClickDown)
            {
                field.OnRightClick(hoveredMine);
                soundPlayer?.Play("mine_right_click");
            }
        }
    }

    void OnBoxRefilled() => soundPlayer?.Play("box_interact");
    void OnFlagPlaced() => soundPlayer?.Play("flag_place");
    void OnFlagRemoved() => soundPlayer?.Play("flag_remove");
}