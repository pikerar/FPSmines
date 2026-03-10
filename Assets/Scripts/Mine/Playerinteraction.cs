using UnityEngine;

/// <summary>
/// Прикрепи к камере игрока (или к самому игроку с камерой).
/// Обрабатывает ЛКМ/ПКМ по минам и наведение на объекты.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Дистанция луча")]
    public float rayDistance = 10f;

    [Header("Камера (если не назначена — ищет Camera.main)")]
    public Camera playerCamera;

    // Ссылки для передачи событий
    private Minefield minefield;

    // Текущий объект под прицелом
    private MineCell hoveredMine;
    private FlagBox hoveredBox;

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

    // -------------------------------------------------------
    // Рейкаст каждый кадр
    // -------------------------------------------------------
    void HandleRaycast()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        MineCell newMine = null;
        FlagBox newBox = null;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            newMine = hit.collider.GetComponentInParent<MineCell>();
            if (newMine == null) newBox = hit.collider.GetComponentInParent<FlagBox>();
        }

        // Обновляем hover-состояние
        if (newMine != hoveredMine)
        {
            hoveredMine = newMine;
        }
        if (newBox != hoveredBox)
        {
            hoveredBox = newBox;
        }

        // Сообщаем HUD что под прицелом
        MinefieldHUD.Instance?.UpdateHoverHint(hoveredMine, hoveredBox);
    }

    // -------------------------------------------------------
    // Клики
    // -------------------------------------------------------
    void HandleInput()
    {
        if (hoveredMine != null)
        {
            if (Input.GetMouseButtonDown(0)) // ЛКМ
            {
                minefield?.OnLeftClick(hoveredMine);
            }
            else if (Input.GetMouseButtonDown(1)) // ПКМ
            {
                minefield?.OnRightClick(hoveredMine);
            }
        }
    }
}