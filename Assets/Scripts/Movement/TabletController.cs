using UnityEngine;

public class TabletController : MonoBehaviour
{
    public GameObject tabletObject;  // ССЫЛКА на планшет, который уже на сцене
    public Transform holdPoint;      // точка удержания перед персонажем

    private bool tabletActive = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    void Start()
    {
        // Сохраняем оригинальное положение планшета
        if (tabletObject != null)
        {
            originalPosition = tabletObject.transform.position;
            originalRotation = tabletObject.transform.rotation;
            originalParent = tabletObject.transform.parent;
            tabletObject.SetActive(false);  // спрятать изначально
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleTablet();
        }
    }

    void ToggleTablet()
    {
        if (!tabletActive)
        {
            // Показать планшет в руках
            ShowTabletInHand();
        }
        else
        {
            // Спрятать планшет на место
            HideTablet();
        }
    }

    void ShowTabletInHand()
    {
        if (tabletObject != null)
        {
            tabletObject.SetActive(true);
            tabletObject.transform.position = holdPoint.position;
            tabletObject.transform.rotation = holdPoint.rotation;
            tabletObject.transform.SetParent(holdPoint);
            tabletActive = true;

            
        }
    }

    void HideTablet()
    {
        if (tabletObject != null)
        {
            // Возвращаем на оригинальное место
            tabletObject.transform.SetParent(originalParent);
            tabletObject.transform.position = originalPosition;
            tabletObject.transform.rotation = originalRotation;
            tabletObject.SetActive(false);
            tabletActive = false;

        }
    }
}
