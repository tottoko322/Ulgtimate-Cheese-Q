using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum LocomotionState 
    { 
        Grounded,
        Airborne,
        WallCling
    } 

    public LocomotionState CurrentLocomotionState { get; private set; } = LocomotionState.Grounded;

    [Header("Ground Movement")]
    [SerializeField] private float groundMoveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 5f; 
    [SerializeField] private float airJumpPower = 5f;
    [SerializeField] private float maxJumpHoldTime = 0.2f;
    [SerializeField] private float jumpHoldForce = 5f;
    private float jumpHoldTimer = 0f;

    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed = 8f; 
    [SerializeField] private float airAcceleration = 3f;
    [SerializeField] private float airDeceleration = 1.5f;

    [Header("Fast Fall")]
    [SerializeField] private float fastFallSpeed = 8f;

    private bool jumpPressed = false;
    private bool hasUsedAirJump = false;

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
        if (CurrentLocomotionState == LocomotionState.Grounded)
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
        if (CurrentLocomotionState == LocomotionState.Airborne)
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
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0f, airDeceleration*Time.fixedDeltaTime), rb.linearVelocity.y);
                break;

                case 6:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, airMoveSpeed, airAcceleration*Time.fixedDeltaTime), rb.linearVelocity.y);
                break;
            }
        }
    }

    private void StartFastFall()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame && CurrentLocomotionState == LocomotionState.Airborne )
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -fastFallSpeed));
    }

    private void Jump()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y -0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, Ground);
        if (hit.collider != null && rb.linearVelocity.y <= 0)
        {
            CurrentLocomotionState = LocomotionState.Grounded;
            hasUsedAirJump = false;
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            switch(CurrentLocomotionState)
            {
                case LocomotionState.Grounded:
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                CurrentLocomotionState = LocomotionState.Airborne;
                jumpPressed = true;
                break;

                case LocomotionState.Airborne:
                if (hasUsedAirJump != true)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, airJumpPower);
                        hasUsedAirJump = true;
                        jumpPressed = true;
                        break;
                    }
                else
                    {
                        break;
                    }
            }
        }

        if (Keyboard.current.wKey.wasReleasedThisFrame)
        {
            jumpPressed = false;
            jumpHoldTimer = 0f;
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.4f);
            }
        }

        if (jumpPressed && Keyboard.current.wKey.isPressed && jumpHoldTimer < maxJumpHoldTime)
        {
            rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
            jumpHoldTimer += Time.fixedDeltaTime;
        }
    }
}
