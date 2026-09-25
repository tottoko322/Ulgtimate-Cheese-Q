using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum LocomotionState 
    { 
        Grounded,
        Airborne,
        WallCling,
        ClimbingLedge
    } 

    public LocomotionState CurrentLocomotionState { get; private set; } = LocomotionState.Grounded;

    [Header("Ground Movement")]
    [SerializeField] private float groundMoveSpeed;

    [Header("Jump")]
    [SerializeField] private float jumpPower; 
    [SerializeField] private float airJumpPower;
    [SerializeField] private float fallPower;
    [SerializeField] private float maxJumpHoldTime;
    [SerializeField] private float jumpHoldForce;
    [SerializeField] private float rayLength;
    private float jumpHoldTimer = 0f;
    private float ledgeJumpTimer = 0f;
    private float wallKickTimer = 10f;

    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed; 
    [SerializeField] private float airAcceleration;
    [SerializeField] private float airDeceleration;

    [Header("Fast Fall")]
    [SerializeField] private float fastFallSpeed;
    [SerializeField] private float slowDownFallMultiplier;

    [Header("Wall")]
    [SerializeField] private float wallMoveSpeed;
    [SerializeField] private float wallDetachForce;
    [SerializeField] private float wallKickForce;
    [SerializeField] private float wallKickLimitTime;
    [SerializeField] private LayerMask climbableWallLayer;

    [Header("Ledge Climb")] 
    [SerializeField] private float ledgeClimbUpSpeed; 
    [SerializeField] private float ledgeClimbForwardSpeed;
    [SerializeField] private float ledgeClimbFallPower;
    private float ledgeClimbDuration;

    [Header("Ledge Jump")] 
    [SerializeField] private float ledgeJumpPower;
    [SerializeField] private float ledgeJumpHorizontalPower;
    [SerializeField] private float ledgeJumpInputWindow; //ledgejumpの受け入れ時間

    private int wallDirection = 0;

    private Vector2 moveInput;

    private bool jumpPressed;
    private bool jumpHeld;
    private bool hasUsedAirJump;
    private bool airJumpStateBeforeFastFalling;
    private bool isFastFalling;
    private bool stopFastFalling;
    private bool isAtLedge;

    private bool isGrounded;
    private bool isRightTouchingWall;
    private bool isLeftTouchingWall;
    private bool canLedgeClimb;
    private bool hasLedgeJumped;

    private bool wallKick;

    [SerializeField] private LayerMask groundLayer;
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
        UpdateWallState();
        UpdateLocomotionState();

        HandleGroundedMovement();
        HandleAirMovement();

        Jump();
        StartFastFall();

        EnterWallCling();
        HandleWallMovement();
        DetachFromWall();
        StartLedgeClimb();
        HandleLedgeClimb();
        CompleteLedgeClimb();
        StartLedgeJump();
        //Debug.Log(CurrentLocomotionState);
    }

    private void ReadInput() //入力取得
    {
        //左右移動、ジャンプ、壁離れの入力
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
        if (CurrentLocomotionState == LocomotionState.Grounded || CurrentLocomotionState == LocomotionState.Airborne || CurrentLocomotionState == LocomotionState.ClimbingLedge)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpPressed = true;
            }

            jumpHeld = Keyboard.current.spaceKey.isPressed;
        }

        //壁キックの入力
        if (Keyboard.current.spaceKey.wasPressedThisFrame && (CurrentLocomotionState == LocomotionState.WallCling || wallKickTimer < wallKickLimitTime))
        {
            wallKick = true;
        }

        //急降下の入力
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            isFastFalling = true;
            airJumpStateBeforeFastFalling = hasUsedAirJump; //急降下前の空中ジャンプの使用の有無の記録
        }
        if (Keyboard.current.wKey.wasPressedThisFrame && isFastFalling)
        {
            stopFastFalling = true;
        }
    }

    private void UpdateGroundState() //接地判定
    {
        Vector2 leftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y -0.5f);
        Vector2 rightRayOrigin = new Vector2(transform.position.x +0.5f, transform.position.y -0.5f);

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength, groundLayer); //下面から下向きに2本

        isGrounded = leftHit.collider != null || rightHit.collider != null; //どちらか片方が接地するとisGroundedがtrueへ
    }

    private void UpdateWallState() //壁の接触判定
    {
        Vector2 topLeftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y +0.5f); //左上
        Vector2 topRightRayOrigin = new Vector2(transform.position.x +0.5f, transform.position.y +0.5f); //右上
        Vector2 bottomLeftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y -0.5f); //左下
        Vector2 bottomRightRayOrigin = new Vector2(transform.position.x +0.5f, transform.position.y -0.5f); //右下

        RaycastHit2D topLeftHit = Physics2D.Raycast(topLeftRayOrigin, Vector2.left, 0.1f, climbableWallLayer); //左上から左へ
        RaycastHit2D topRightHit = Physics2D.Raycast(topRightRayOrigin, Vector2.right, 0.1f, climbableWallLayer); //右上から右へ
        RaycastHit2D bottomLeftHit = Physics2D.Raycast(bottomLeftRayOrigin, Vector2.left, 0.1f, climbableWallLayer); //左下から左へ
        RaycastHit2D bottomRightHit = Physics2D.Raycast(bottomRightRayOrigin, Vector2.right, 0.1f, climbableWallLayer); //右下から右へ

        isRightTouchingWall = (topRightHit.collider != null && bottomRightHit.collider != null);
        isLeftTouchingWall = (topLeftHit.collider != null && bottomLeftHit.collider != null);

        isAtLedge = ((topLeftHit.collider == null && bottomLeftHit.collider != null) || (topRightHit.collider == null && bottomRightHit.collider != null));
        //上端が壁に当たっていないかつ下端が壁に当たっているならば、壁上端
    }

    private void UpdateLocomotionState() //足場状態更新
    {
        if (isGrounded) //接地ならば足場状態Groundedへ
        {
            CurrentLocomotionState = LocomotionState.Grounded;
            hasUsedAirJump = false; //空中ジャンプの復活
            isFastFalling = false; //急降下の復活
            wallKickTimer = wallKickLimitTime;
        }
        else if (isRightTouchingWall || isLeftTouchingWall) //isTouchingWallがtrueならば足場状態WallClingへ
        {
            CurrentLocomotionState = LocomotionState.WallCling;
        }
        else //isGrounded,isTouchingWallがfalseならば足場状態Airborneへ
        {
            CurrentLocomotionState = LocomotionState.Airborne;
        }

        if (canLedgeClimb)
        {
            CurrentLocomotionState = LocomotionState.ClimbingLedge;
        }
        if (hasLedgeJumped)
        {
            CurrentLocomotionState = LocomotionState.Airborne;
            hasLedgeJumped = false;
        }
    }

    private void HandleGroundedMovement() //地面での左右移動
    {
        if (CurrentLocomotionState == LocomotionState.Grounded)
        {
            rb.linearVelocity = new Vector2(groundMoveSpeed * moveInput.x, rb.linearVelocity.y);
        }
    }

    private void HandleAirMovement() //空中移動
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

            if (rb.linearVelocity.y < 0f && isFastFalling != true) //落下速度の調節
            {
                rb.AddForce(Vector2.down * fallPower, ForceMode2D.Force);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -fastFallSpeed));
            }
        }
    }

    private void Jump() //ジャンプ
    {
        if (CurrentLocomotionState == LocomotionState.Grounded || CurrentLocomotionState == LocomotionState.Airborne)
        {
            if (jumpPressed)
            {
                switch(CurrentLocomotionState)
                {
                    case LocomotionState.Grounded: //一回目のジャンプ
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);

                    jumpHoldTimer = 0f;
                    jumpPressed = false;
                    break;

                    case LocomotionState.Airborne:
                    if (hasUsedAirJump == false && wallKickTimer >= wallKickLimitTime) //二回目のジャンプ
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, airJumpPower);

                        jumpHoldTimer = 0f;
                        jumpPressed = false;
                        hasUsedAirJump = true;

                        ledgeJumpTimer = 0f; //
                        break;
                    }
                    else //それ以降
                    {
                        jumpPressed = false;
                        break;
                    }
                }
            }

            if (jumpHeld && jumpHoldTimer < maxJumpHoldTime && rb.linearVelocity.y > 0) //高さ調節
            {
                //Debug.Log("高さ調節");
                rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
                jumpHoldTimer += Time.fixedDeltaTime;
            }
        }
    }

    private void StartFastFall() //急降下について
    {
        //Debug.Log(stopFastFalling);
        if (isFastFalling && CurrentLocomotionState == LocomotionState.Airborne) //急降下
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -fastFallSpeed));
            hasUsedAirJump = true;//急降下中のジャンプを無くす

            //Debug.Log(stopFastFalling);

            if (stopFastFalling) //急降下の解除
            {
                //Debug.Log("stopFastFalling");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * slowDownFallMultiplier);

                isFastFalling = false;
                stopFastFalling = false;

                if (airJumpStateBeforeFastFalling == false) //空中ジャンプが使用されていないとき、空中ジャンプの回復
                {
                    hasUsedAirJump = false;
                    Debug.Log("ジャンプ回復");
                }
            }
        }
    }

    private void EnterWallCling() //重力操作、壁張り付きで空中ジャンプと急降下回復
    {
        if (CurrentLocomotionState == LocomotionState.WallCling)
        {
            rb.gravityScale = 0f;
            hasUsedAirJump = false; //空中ジャンプ回復
            isFastFalling = false; //急降下回復
            wallKickTimer = 0f;

            if (isRightTouchingWall) //右の壁なら1
            {
                wallDirection = 1;
            }
            if (isLeftTouchingWall) //左の壁なら-1
            {
                wallDirection = -1;
            }
        }
        else
        {
            rb.gravityScale = 1f;
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

    private void DetachFromWall() //壁から離れるor壁キック
    {
        if (CurrentLocomotionState == LocomotionState.WallCling && moveInput.x != 0) //壁から離れる
        {
            float velocityX = moveInput.x * wallDetachForce;
            rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y * 0.3f);

            wallKickTimer += Time.fixedDeltaTime;
        }

        if (wallKick && moveInput.y >= 0 && wallKickTimer < wallKickLimitTime) //壁キック
        {
            float velocityX = -wallDirection * wallDetachForce;
            rb.linearVelocity = new Vector2(velocityX, wallKickForce);

            wallKick = false;
            wallKickTimer = wallKickLimitTime;
            Debug.Log("壁キック");
        }
        else if (wallKick && moveInput.y < 0)
        {
            wallKick = false;
        }
    }

    private void StartLedgeClimb() //ClimbongLedgeへ状態変化
    {
        if (CurrentLocomotionState == LocomotionState.Airborne && isAtLedge && moveInput.y > 0)
        {
            canLedgeClimb = true;
        }
    }

    private void HandleLedgeClimb() //よじ登り小ジャンプ
    {
        if (hasLedgeJumped)
        {
            return;
        }

        if (CurrentLocomotionState == LocomotionState.ClimbingLedge)
        {
            ledgeClimbDuration += Time.fixedDeltaTime; //上方向に速度を加える時間のタイマー
 
            if (ledgeClimbDuration < 0.25) //ある一定時間まで上方向に上昇
            {
                rb.linearVelocity = new Vector2(0f, ledgeClimbUpSpeed);
            }
            else if (0.25 <= ledgeClimbDuration && ledgeClimbDuration < 0.4)
            {
                rb.linearVelocity = new Vector2(wallDirection * ledgeClimbForwardSpeed, rb.linearVelocity.y);
            }
        }
    }

    private void CompleteLedgeClimb()
    {
        if (isGrounded)
        {
            canLedgeClimb = false; //壁よじ登りの終わり
            hasLedgeJumped = false; //ledgejumpの回復
            ledgeClimbDuration = 0f; //壁よじ登りのタイマー0へ
        }
    }

    private void StartLedgeJump()
    {
        if (CurrentLocomotionState == LocomotionState.ClimbingLedge)
        {
            ledgeJumpTimer += Time.fixedDeltaTime;
        }

        if (0.3f < ledgeJumpTimer && ledgeJumpTimer < ledgeJumpInputWindow && jumpPressed)
        {
            rb.linearVelocity = new Vector2(-wallDirection * ledgeJumpHorizontalPower, ledgeJumpPower);

            hasLedgeJumped = true;
            canLedgeClimb = false;

            ledgeClimbDuration = 0f;
            ledgeJumpTimer = 0f;

            //通常ジャンプをした後と同じ状況にする
            jumpPressed = false;
            jumpHeld = false;
        }
    }
}
