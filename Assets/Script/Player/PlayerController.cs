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
    [SerializeField] private float groundMoveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 5f; 
    [SerializeField] private float airJumpPower = 5f;
    [SerializeField] private float maxJumpHoldTime = 0.2f;
    [SerializeField] private float jumpHoldForce = 5f;
    private float jumpHoldTimer = 0f;

    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed = 6f; 
    [SerializeField] private float airAcceleration = 30f;
    [SerializeField] private float airDeceleration = 5f;

    [Header("Fast Fall")]
    [SerializeField] private float fastFallSpeed = 6f;

    [Header("Wall")]
    [SerializeField] private float wallMoveSpeed;
    [SerializeField] private float wallDetachForce;
    [SerializeField] private LayerMask climbableWallLayer;

    private Vector2 moveInput;
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
        Jump();
        StartFastFall();
        EnterWallCling();
        HandleWallMovement();
    }

    private void ReadInput()
    {
        //移動の入力
        moveInput = Vector2.zero;
        if (Keyboard.current.dKey.isPressed)
        {
            moveInput.x += 1;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            moveInput.x -= 1;
        }
        if (Keyboard.current.wKey.isPressed)
        {
            moveInput.y += 1;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            moveInput.y -= 1;
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

        //急降下の入力
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            isFastFalling = true;
        }
    }

    private void UpdateGroundState() //接地判定
    {
        Vector2 leftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y -0.5f);
        Vector2 rightRayOrigin = new Vector2(transform.position.x +0.5f, transform.position.y -0.5f);//rayを2本作る

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, 0.1f, Ground);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, 0.1f, Ground);

        if ((leftHit.collider != null || rightHit.collider != null) && rb.linearVelocity.y <= 0)
        {
            CurrentLocomotionState = LocomotionState.Grounded;
            hasUsedAirJump = false;
            isFastFalling = false;
        }

        else
        {
            CurrentLocomotionState = LocomotionState.Airborne;
        }
    }

    private void HandleGroundedMovement() //地面での左右移動
    {
        if (CurrentLocomotionState == LocomotionState.Grounded)
        {
            rb.linearVelocity = new Vector2(groundMoveSpeed * moveInput.x, rb.linearVelocity.y);
        }
    }

    private void HandleAirMovement() //空中での左右移動
    {
        if (CurrentLocomotionState == LocomotionState.Airborne)
        {
            switch(moveInput.x)
            {
                case -1:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, -airMoveSpeed, airAcceleration * Time.fixedDeltaTime), rb.linearVelocity.y);
                break;

                case 0:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0f, airDeceleration * Time.fixedDeltaTime), rb.linearVelocity.y);
                break;

                case 1:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, airMoveSpeed, airAcceleration * Time.fixedDeltaTime), rb.linearVelocity.y);
                break;
            }
        }
    }

    private void Jump() //ジャンプ
    {
        if (jumpPressed ())
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

    private void StartFastFall() //急降下
    {
        if (isFastFalling && CurrentLocomotionState == LocomotionState.Airborne)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -fastFallSpeed));
            hasUsedAirJump = true;//急降下中のジャンプを無くす
        }
    }

    private void EnterWallCling() //壁への接触判定
    {
        Vector2 topLeftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y +0.5f);
        Vector2 bottomLeftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y -0.5f); //左上と左下に1本ずつのray

        RaycastHit2D topLeftHit = Physics2D.Raycast(topLeftRayOrigin, Vector2.left, 0.1f, climbableWallLayer);
        RaycastHit2D bottomLeftHit = Physics2D.Raycast(bottomLeftRayOrigin, Vector2.left, 0.1f, climbableWallLayer); //左方向

        if (CurrentLocomotionState == LocomotionState.Airborne && topLeftHit.collider != null && bottomLeftHit.collider != null)
        {
            CurrentLocomotionState = LocomotionState.WallCling;
        }
    }

    private void HandleWallMovement() //壁での上下移動
    {
        if (CurrentLocomotionState == LocomotionState.WallCling)
        {

            float velocityY = moveInput.y * wallMoveSpeed;
            rb.linearVelocity = new Vector2(0f, velocityY);
        }
    }
}
