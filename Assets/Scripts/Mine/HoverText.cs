using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObject : MonoBehaviour
{
    [Header("Hover Text")]
    public TextMeshProUGUI centerText;
    public string hoverMessage = "Нажмите E для взаимодействия";
    public float maxDistance = 100f;

    [Header("Interaction")]
    public GameObject interactionText3D;
    public string interactionMessage = "Взаимодействие выполнено!";
    public bool isBoom = false;           // галочка для взрыва

    [Header("Tablet UI")]
    public Camera droneCamera;

    //[Header("Drone Text")]
    //public GameObject mineNumDronePrefab; // префаб текста для дрона
    //public float droneTextHeight = 0.5f;  // высота над миной

    public GameObject myDroneText;

    public Camera cam;


    private RectTransform myMineNum;
    private bool isHovered = false;
    private bool gameOver = false;

    void Start()
    {
        if (gameOver) return;
        if (interactionText3D != null)
            interactionText3D.SetActive(false);
        if (myDroneText != null)
            myDroneText.SetActive(false);
    }

    void Update()
    {
        if (isHovered && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void OnMouseOver()
    {
        if (gameOver) return;

        float distance = Vector3.Distance(cam.transform.position, transform.position);
        if (distance <= maxDistance)
        {
            isHovered = true;
            if (centerText != null)
                centerText.text = hoverMessage;
        }
    }

    void OnMouseExit()
    {
        if (gameOver) return;

        isHovered = false;
        if (centerText != null)
            centerText.text = "";
    }

    void Interact()
    {
        if (isBoom)
        {
            Explode();
        }
        else
        {
            ShowInteractionText();
        }
    }

    void ShowInteractionText()
    {
        if (interactionText3D != null)
        {
            interactionText3D.SetActive(true);
            TextMeshPro interactionTMP = interactionText3D.GetComponent<TextMeshPro>();
            if (interactionTMP != null)
                interactionTMP.text = interactionMessage;
        }
        ShowDroneText();
    }

    void ShowDroneText()
    {
        if (myDroneText != null)
        {
            myDroneText.SetActive(true);
            TextMeshPro DroneTMP = myDroneText.GetComponent<TextMeshPro>();
            if (DroneTMP != null)
                DroneTMP.text = interactionMessage;
        }
    }


    void Explode()
    {
        gameOver = true;
        // Звук взрыва
        //AudioSource.PlayClipAtPoint(GetComponent<AudioSource>()?.clip, transform.position);

        // Физический взрыв
        //Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        //foreach (Rigidbody rb in rbs)
        //{
        //    rb.AddExplosionForce(1000f, transform.position, 10f);
        //}

        SceneManager.LoadScene("EndGame");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
