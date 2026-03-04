using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private const float moveSpeed = 5f;
    private const float groundDrag = 8f;

    private const float jumpForce = 5.5f;
    private float jumpCooldown;
    private const float airMultiplier = 0.9f;
    bool readyToJump;

    public float walkSpeed;
    public float sprintSpeed;

    [Header("Keybinds")]
    private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    private float playerHeight = 2;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    private const float speedMultip = 1000f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // так называемый граунд чек
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        // передвижение
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // ввод прыжка
        if(Input.GetKey(jumpKey) && readyToJump && grounded) //надо задуматься о замене кнопки прыжка на константу
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // выбор направления движения по повороту камеры
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // движение на земле
        if(grounded)
            rb.AddForce(speedMultip * moveSpeed * moveDirection.normalized * Time.fixedDeltaTime, ForceMode.Force);

        // движение в воздухе, с дебафом в полете
        else if(!grounded)
            rb.AddForce(speedMultip * moveSpeed * moveDirection.normalized * airMultiplier * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // лимит скорости передвижения по x z коордам
        if(flatVel.magnitude > moveSpeed)
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
}