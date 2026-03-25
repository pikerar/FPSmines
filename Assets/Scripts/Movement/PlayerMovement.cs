using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private const float moveSpeed = 5f;
    private const float groundDrag = 8f;
    private const float jumpForce = 9.0f;
    private const float airMultiplier = 1.3f;
    private const float fallMultiplier = 3f;
    private const float lowJumpMultiplier = 2f;
    private const float speedMultip = 1000f;

    [Header("Jump")]
    private float jumpCooldown = 0.9f;
    private bool readyToJump;

    [Header("Ground Check")]
    private float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("References")]
    public Transform orientation;

    private Rigidbody rb;
    private InputHandler input;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        input = InputHandler.Instance;
        if (input == null)
            Debug.LogError("[PlayerMovement] InputHandler не найден на сцене!");
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        HandleJumpInput();
        SpeedControl();

        rb.linearDamping = grounded ? groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
    }

    // -------------------------------------------------------
    // Прыжок — читаем из InputHandler
    // -------------------------------------------------------
    private void HandleJumpInput()
    {
        if (input.JumpPressed && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    //отдельный скрипт с инпутом
    private void MovePlayer()
    {
        Vector3 moveDirection = orientation.forward * input.Vertical + orientation.right * input.Horizontal;

        float multiplier = grounded ? 1f : airMultiplier;
        rb.AddForce(speedMultip * moveSpeed * multiplier * moveDirection.normalized * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void ApplyGravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.AddForce(Physics.gravity * (fallMultiplier - 1) * rb.mass, ForceMode.Force);
        else if (rb.linearVelocity.y > 0 && !input.JumpHeld)
            rb.AddForce(Physics.gravity * (lowJumpMultiplier - 1) * rb.mass, ForceMode.Force);
    }
}