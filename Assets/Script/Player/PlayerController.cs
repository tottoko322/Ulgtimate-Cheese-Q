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

    [SerializeField] private float groundMoveSpeed = 10f;

    [SerializeField] private float jumpPower = 5f; 
    [SerializeField] private float airJumpPower = 5f;
    [SerializeField] private float maxJumpHoldTime = 0.2f;
    private float jumpHoldTimer = 0f;
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
    }

    void FixedUpdate()
    {
        HandleGroundedMovement();
    }

    private void HandleGroundedMovement()
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
