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
    [SerializeField] private float airAcceleration = 15f;
    [SerializeField] private float airDeceleration = 3f;

    [Header("Fast Fall")]
    [SerializeField] private float fastFallSpeed = 8f;

    private int moveInput;
    private int airMoveInput;
    private bool jumpPressed = false;
    private bool jumpReleased = false;
    private bool jumpHeld = false;
    private bool hasUsedAirJump = false;
    private bool isFastFalling = false;

    [SerializeField] private LayerMask Ground;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        UpdateGroundState();
        HandleGroundedMovement();
        HandleAirMovement();
        StartFastFall();
        Jump();
    }

    private void ReadInput()
    {
        //左右移動の入力
        moveInput = 0;
        if (Keyboard.current.dKey.isPressed)
        {
            moveInput += 1;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            moveInput -= 1;
        }
        
        //急降下の入力
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            isFastFalling = true;
        }

        //ジャンプの入力
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }
        if (Keyboard.current.wKey.wasReleasedThisFrame)
        {
            jumpReleased = true;
        }

        jumpHeld = Keyboard.current.wKey.isPressed;
    }

    private void UpdateGroundState() //接地判定
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y -0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, Ground);
        if (hit.collider != null && rb.linearVelocity.y <= 0)
        {
            CurrentLocomotionState = LocomotionState.Grounded;
            hasUsedAirJump = false;
            isFastFalling = false;
        }
    }

    private void HandleGroundedMovement() //地面での左右移動
    {
        if (CurrentLocomotionState == LocomotionState.Grounded)
        {
            rb.linearVelocity = new Vector2(groundMoveSpeed * moveInput, rb.linearVelocity.y);
        }
    }

    private void HandleAirMovement() //空中での左右移動
    {
        if (CurrentLocomotionState == LocomotionState.Airborne)
        {
            airMoveInput = moveInput + 5;
            switch(airMoveInput)
            {
                case 4:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, -airMoveSpeed, airAcceleration * Time.fixedDeltaTime), rb.linearVelocity.y);
                break;

                case 5:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0f, airDeceleration * Time.fixedDeltaTime), rb.linearVelocity.y);
                break;

                case 6:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, airMoveSpeed, airAcceleration * Time.fixedDeltaTime), rb.linearVelocity.y);
                break;
            }
        }
    }

    private void StartFastFall() //急降下
    {
        if (isFastFalling && CurrentLocomotionState == LocomotionState.Airborne)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -fastFallSpeed));
    }

    private void Jump() //ジャンプ
    {
        if (jumpPressed)
        {
            switch(CurrentLocomotionState)
            {
                case LocomotionState.Grounded: //一回目のジャンプ
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                CurrentLocomotionState = LocomotionState.Airborne;
                jumpHoldTimer = 0f;
                jumpPressed = false;
                break;

                case LocomotionState.Airborne:
                if (hasUsedAirJump == false) //二回目のジャンプ
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, airJumpPower);
                        jumpHoldTimer = 0f;
                        jumpPressed = false;
                        hasUsedAirJump = true;
                        break;
                    }
                else //それ以降
                    {
                        jumpPressed = false;
                        break;
                    }
            }
        }

        if (jumpReleased)
        {
            jumpHoldTimer = 0;
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.4f);
            }
            jumpReleased = false;
        }

        if (jumpHeld && jumpHoldTimer < maxJumpHoldTime)
        {
            rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
            jumpHoldTimer += Time.fixedDeltaTime;
        }
    }
}
