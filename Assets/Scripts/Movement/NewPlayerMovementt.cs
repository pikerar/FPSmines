using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerMovement: MonoBehaviour
{

    public CharacterController controller;
    public Transform GroundCheck;
    public LayerMask groundMask;
    public float speed = 3f;
    public float gravity = 9.8f;
    public float tabletHideDelay = 0.5f;

    public float jumpHeight = 3f;
    public float groundDistance = 0.4f;
    public Animator anim;
    public GameObject cam;
    public GameObject maincam;
    public GameObject tablet;
    
    public Vector3 velocity;
    public Vector3 move;
    public bool isGrounded;

    public bool InputLocked { get; set; }

    private Coroutine hideCoroutine;
    private bool tabletOut = false;

    private CameraLook cameraLook;
    private float defaultMaxY;
    // Start is called before the first frame update
    void Start()
    {
        cameraLook = GetComponent<CameraLook>(); 
        defaultMaxY = cameraLook.maxY;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputLocked)
        {
            anim.SetFloat("X", 0f);
            anim.SetFloat("Y", 0f);
            return;
        }

        if (controller.isGrounded)
        {
            anim.SetFloat("X", Input.GetAxis("Horizontal"));
            anim.SetFloat("Y", Input.GetAxis("Vertical"));

        }
        
        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tabletOut = !tabletOut;
                if (tabletOut)
                {
                    if (hideCoroutine != null) StopCoroutine(hideCoroutine);
                    tablet.SetActive(true);
                }
                else
                {
                    if (hideCoroutine != null) StopCoroutine(hideCoroutine);
                    hideCoroutine = StartCoroutine(HideTabletDelayed());
                }
            }
                

            float current = anim.GetLayerWeight(1);
            float target = tabletOut ? 1f : 0f;
            anim.SetLayerWeight(1, Mathf.MoveTowards(current, target, Time.deltaTime * 5f));
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            cameraLook.maxY = 40f;
            velocity.y = Mathf.Sqrt(jumpHeight * gravity);
            anim.SetTrigger("Jump");
        }

        if (isGrounded)
        {
            cameraLook.maxY = defaultMaxY;
        }

        move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        if (Input.GetKey("c"))
        {
            controller.height = 1f;
        }
        
        else
        {
            controller.height = 1.79f;
        }
        
        if (Input.GetKey("left shift"))
        {
            speed = 7f;
            anim.SetFloat("Y", 2);
            cam.SetActive(true);
            maincam.SetActive(false);
        }
        
        else
        {
            speed = 3f;
            cam.SetActive(false);
            maincam.SetActive(true);
        }
    }
    private IEnumerator HideTabletDelayed()
    {
        yield return new WaitForSeconds(tabletHideDelay);
        tablet.SetActive(false);
    }
}
