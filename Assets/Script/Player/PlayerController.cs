using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum LocomotionState 
    { 
        Grounded,
        Airborne,
        DoubleJump,
        WallCling
    } 

    private LocomotionState jumpState = LocomotionState.Grounded;
    public LocomotionState CurrentLocomotionState { get; private set; } 

    [Header("Ground Movement")] 
    [SerializeField] private float groundMoveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 5f; 
    [SerializeField] private float airJumpPower = 5f;
    [SerializeField] private float maxJumpHoldTime = 0.2f;
    private float jumpHoldTimer = 0f;

    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed = 8f; 
    [SerializeField] private float airAcceleration = 3f; 
    [SerializeField] private float airDeceleration; 

    [Header("Fast Fall")]
    [SerializeField] private float fastFallSpeed = 8f;

    private bool jumpPressed = false;

    [SerializeField] private LayerMask Ground;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Jump();
        HandleAirMovement();
        StartFastFall();
    }

    void FixedUpdate()
    {
        HandleGroundedMovement();
    }

    private void HandleGroundedMovement()
    {
        if (jumpState == LocomotionState.Grounded)
        {
            int playerState = 5;
            if (Keyboard.current.dKey.isPressed)
            {
                playerState += 1;
            }
            if (Keyboard.current.aKey.isPressed)
            {
                playerState -= 1;
            }

            switch(playerState)
            {
                case 4:
                rb.linearVelocity = new Vector2(-groundMoveSpeed, rb.linearVelocity.y);
                break;

                case 5:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

                case 6:
                rb.linearVelocity = new Vector2(groundMoveSpeed, rb.linearVelocity.y);
                break;
            }
        }
    }

    private void HandleAirMovement()
    {
        if (jumpState == LocomotionState.Airborne || jumpState == LocomotionState.DoubleJump)
        {
            int playerAirState = 5;
            if (Keyboard.current.dKey.isPressed)
            {
                playerAirState += 1;
            }
            if (Keyboard.current.aKey.isPressed)
            {
                playerAirState -= 1;
            }
            switch(playerAirState)
            {
                case 4:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, -airMoveSpeed, airAcceleration*Time.fixedDeltaTime), rb.linearVelocity.y);
                break;

                case 5:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

                case 6:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, airMoveSpeed, airAcceleration*Time.fixedDeltaTime), rb.linearVelocity.y);
                break;
            }
        }
    }

    private void StartFastFall()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame && (jumpState == LocomotionState.Airborne || jumpState == LocomotionState.DoubleJump))
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -fastFallSpeed));
    }

    private void Jump()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y -0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, Ground);
        if (hit.collider != null && rb.linearVelocity.y <= 0)
        {
            jumpState = LocomotionState.Grounded;
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            switch(jumpState)
            {
                case LocomotionState.Grounded:
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpState = LocomotionState.Airborne;
                jumpPressed = true;
                break;

                case LocomotionState.Airborne:
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, airJumpPower);
                jumpState = LocomotionState.DoubleJump;
                jumpPressed = true;
                break;

                case LocomotionState.DoubleJump:
                break;
            }
        }

        if (Keyboard.current.wKey.wasReleasedThisFrame)
        {
            jumpPressed = false;
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.4f);
            }
        }

        if (jumpPressed && Keyboard.current.wKey.isPressed && jumpHoldTimer < maxJumpHoldTime)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, airJumpPower);
            jumpHoldTimer += Time.fixedDeltaTime;
        }
    }
}
