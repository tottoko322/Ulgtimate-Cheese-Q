using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageReceiver
{
    public enum LocomotionState
    {
        Grounded,
        Airborne,
        WallCling,
        ClimbingLedge
    }

    public enum ActionState
    {
        Nomal,
        Attacking,
        Hurt,
        Dead
    }

    public LocomotionState CurrentLocomotionState { get; private set; } = LocomotionState.Grounded;
    public ActionState CurrentActionState { get; private set; } = ActionState.Nomal;


    [Header("Ground Movement")]
    [SerializeField] private float groundMoveSpeed; //地上での左右移動の速度

    [Header("Jump")]
    [SerializeField] private float jumpPower; //ジャンプ時に上向きに加える力の大きさ
    [SerializeField] private float airJumpPower; //空中ジャンプ時に上向きに加える力の大きさ
    [SerializeField] private float fallPower; //ジャンプボタンを離したときに下向きに加える力の大きさ
    [SerializeField] private float maxJumpHoldTime; //ジャンプの高さ調節時に上向きに力を与え続ける時間(値が大きいと高く、長く飛ぶ)
    [SerializeField] private float jumpHoldForce; //ジャンプの高さ調節時に上向きに加える力の大きさ
    [SerializeField] private float boxRayLength; //地上ジャンプの受け入れためのbox(プレイヤーの下面から出ている)の大きさ
    private float jumpHoldTimer = 0f;
    private float ledgeJumpTimer = 0f;

    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed; //空中でのmax移動速度
    [SerializeField] private float airAcceleration; //max移動速度への加速度
    [SerializeField] private float airDeceleration; //空気抵抗、慣性みたいなもの

    [Header("Fast Fall")]
    [SerializeField] private float fastFallSpeed; //急降下時の速度
    [SerializeField, Range(0, 1)] private float slowDownFallMultiplier; //急降下キャンセル時の速度の落とし具合(0~1の値。1でそのままの速度)

    [Header("Wall")]
    [SerializeField] private float wallMoveSpeed; //壁での上下移動の速度
    [SerializeField] private float wallDetachForce; //壁から離れるときに与える力の大きさ
    [SerializeField] private float wallKickForce; //壁キック時に上向きに与える力の大きさ
    [SerializeField] private float wallRayLength; //壁キックを受け入れるためのrayの長さ
    [SerializeField, Range(0, 1)] private float detachWallMultiplier; //壁から離れるときのプレイヤーのy方向の速度の落とし具合(0~1の値。1でそのままの速度)
    [SerializeField] private LayerMask climbableWallLayer;

    [Header("Ledge Climb")] 
    [SerializeField] private float ledgeClimbUpSpeed; //崖のぼりの時に上向きに加える力の大きさ
    [SerializeField] private float ledgeClimbForwardSpeed; //崖のぼりの時に横向きに加える力の大きさ
    [SerializeField] private float ledgeJumpUpTime; //崖のぼりの時の上昇する時間
    [SerializeField] private float ledgeJumpHorizontalTime; //崖のぼりの時の横向きに移動する時間
    private float ledgeClimbDuration;

    [Header("Ledge Jump")] 
    [SerializeField] private float ledgeJumpPower; //崖のぼりキャンセル時に上向きに加える力の大きさ
    [SerializeField] private float ledgeJumpHorizontalPower; //崖のぼりキャンセル時に横向きに加える力の大きさ
    [SerializeField] private float ledgeJumpInputWindow; //ledgejumpの受け入れ時間

    [Header("Attack")]
    [SerializeField] private float playerHp; //プレイヤーのHP
    [SerializeField] private float attackRange; //攻撃範囲(円)の半径の大きさ
    [SerializeField] private Vector2 attackPosition; //攻撃範囲(円)の中心位置
    [SerializeField] private float playerAttackDamage; //プレイヤーが与えるダメージの大きさ
    [SerializeField] private Vector2 playerGiveKnockBack; //プレイヤーが与えるノックバックの大きさ
    [SerializeField] private float playerAttackInvincibleTime; //
    [SerializeField] private LayerMask enemyLayer; //

    private int wallDirection = 0;

    private Vector2 moveInput;

    private bool jumpPressed;
    private bool jumpReleased;
    private bool canGroundJump;
    private bool jumpHeld;
    private bool hasUsedAirJump;
    private bool airJumpStateBeforeFastFalling;
    private bool isFastFalling;
    private bool stopFastFalling;
    private bool isAtLeftLedge;
    private bool isAtRightLedge;

    private bool isGrounded;
    private bool isRightTouchingWall;
    private bool isLeftTouchingWall;
    private bool canLedgeClimb;
    private bool hasLedgeJumped;

    private bool isWallKick;
    private bool canWallKick;
    private bool wallKickRange;

    private bool isKnockBack;
    private bool isAttack;
    private bool hasAttacked;

    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;


    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap keyControl;

    private InputAction attack;
    private InputAction moveRight;
    private InputAction moveLeft;
    private InputAction jump;
    private InputAction wallUp;
    private InputAction wallDown;
    private InputAction wallKick;
    private InputAction fastFall;
    private InputAction stopFastFall;

    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        keyControl = inputActions.FindActionMap("KeyControl", true);

        attack = keyControl.FindAction("Attack", true);
        moveRight = keyControl.FindAction("MoveRight", true);
        moveLeft = keyControl.FindAction("MoveLeft", true);
        jump = keyControl.FindAction("Jump", true);
        wallUp = keyControl.FindAction("WallUp", true);
        wallDown = keyControl.FindAction("WallDown", true);
        wallKick = keyControl.FindAction("WallKick", true);
        fastFall = keyControl.FindAction("FastFall", true);
        stopFastFall = keyControl.FindAction("StopFastFall", true);
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

        //Debug.Log(isAttack);
        UpdateActionState();

        Attack();

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
        //Debug.Log($"isAttack: {isAttack}, ActionState: {CurrentActionState}");
        Debug.Log(CurrentLocomotionState);
        //Debug.Log(CurrentActionState);
        //Debug.Log(previousState);
        //Debug.Log(wallDirection);
        //Debug.Log(isAtLeftLedge);
        //Debug.Log(canGroundJump);
        //Debug.Log(isAttack);

        UpdateAnimation();
    }

    private void ReadInput() //入力取得
    {
        if (CurrentActionState == ActionState.Dead || isKnockBack) //死亡orノックバックで入力できない
        {
            return;
        }

        if (attack.WasPressedThisFrame())
        {
            isAttack = true;

            hasAttacked = false;
        }

        //左右移動、ジャンプ、壁離れの入力
        moveInput = Vector2.zero;
        if (moveRight.IsPressed())
        {
            moveInput.x += 1;
        }
        if (moveLeft.IsPressed())
        {
            moveInput.x -= 1;
        }
        if (wallUp.IsPressed())
        {
            moveInput.y += 1;
        }
        if (wallDown.IsPressed())
        {
            moveInput.y -= 1;
        }

        //ジャンプの入力
        if (canGroundJump || CurrentLocomotionState == LocomotionState.Airborne || CurrentLocomotionState == LocomotionState.ClimbingLedge)
        {
            if (jump.WasPressedThisFrame())
            {
                jumpPressed = true;
            }

            jumpHeld = jump.IsPressed();

            if (jump.WasReleasedThisFrame())
            {
                jumpReleased = true;
            }
        }

        //壁キックの入力
        if (wallKick.WasPressedThisFrame() && CurrentLocomotionState == LocomotionState.WallCling)
        {
            isWallKick = true;
        }
        if (wallKick.WasPressedThisFrame() && CurrentLocomotionState == LocomotionState.Airborne && wallKickRange)
        {
            Debug.Log("canWallKick");
            canWallKick = true;
        }

        //急降下の入力
        if (fastFall.WasPressedThisFrame())
        {
            isFastFalling = true;
            airJumpStateBeforeFastFalling = hasUsedAirJump; //急降下前の空中ジャンプの使用の有無の記録
        }
        if (stopFastFall.WasPressedThisFrame() && isFastFalling)
        {
            stopFastFalling = true;
        }
    }

    private void UpdateGroundState() //接地判定
    {
        if (isKnockBack)
        {
            return;
        }
        Vector2 leftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y -0.5f);
        Vector2 rightRayOrigin = new Vector2(transform.position.x +0.5f, transform.position.y -0.5f);

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, 0.1f, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, 0.1f, groundLayer); //下面から下向きに2本

        isGrounded = leftHit.collider != null || rightHit.collider != null; //どちらか片方が接地すると地面


        RaycastHit2D groundhit = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0f, Vector2.down, boxRayLength, groundLayer);
        //地上ジャンプの受け入れに使用
        canGroundJump = groundhit.collider != null;
    }

    private void UpdateWallState() //壁の接触判定
    {
        if (isKnockBack)
        {
            return;
        }
        Vector2 topLeftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y +0.5f); //左上
        Vector2 topRightRayOrigin = new Vector2(transform.position.x +0.5f, transform.position.y +0.5f); //右上
        Vector2 bottomLeftRayOrigin = new Vector2(transform.position.x -0.5f, transform.position.y -0.5f); //左下
        Vector2 bottomRightRayOrigin = new Vector2(transform.position.x +0.5f, transform.position.y -0.5f); //右下

        RaycastHit2D topLeftHit = Physics2D.Raycast(topLeftRayOrigin, Vector2.left, 0.1f, climbableWallLayer); //左上から左へ
        RaycastHit2D topRightHit = Physics2D.Raycast(topRightRayOrigin, Vector2.right, 0.1f, climbableWallLayer); //右上から右へ
        RaycastHit2D bottomLeftHit = Physics2D.Raycast(bottomLeftRayOrigin, Vector2.left, 0.1f, climbableWallLayer); //左下から左へ
        RaycastHit2D bottomRightHit = Physics2D.Raycast(bottomRightRayOrigin, Vector2.right, 0.1f, climbableWallLayer); //右下から右へ

        Vector2 halfRayOrigin = new Vector2(transform.position.x +0.5f * wallDirection, transform.position.y);
        RaycastHit2D halftHit = Physics2D.Raycast(halfRayOrigin, Vector2.right * wallDirection, wallRayLength, climbableWallLayer);

        wallKickRange = halftHit.collider != null;

        isRightTouchingWall = (topRightHit.collider != null && bottomRightHit.collider != null);
        isLeftTouchingWall = (topLeftHit.collider != null && bottomLeftHit.collider != null);

        isAtLeftLedge = topLeftHit.collider == null && bottomLeftHit.collider != null;
        isAtRightLedge = topRightHit.collider == null && bottomRightHit.collider != null;
        //上端が壁に当たっていないかつ下端が壁に当たっているならば、壁上端
    }

    private void UpdateLocomotionState() //足場状態更新
    {
        if (isGrounded) //接地ならば足場状態Groundedへ
        {
            CurrentLocomotionState = LocomotionState.Grounded;
            hasUsedAirJump = false; //空中ジャンプの復活
            isFastFalling = false; //急降下の復活
            wallDirection = 0;

            isKnockBack = false; //地面につくとノックバック終わり
        }
        else if (isRightTouchingWall || isLeftTouchingWall) //isTouchingWallがtrueならば足場状態WallClingへ
        {
            CurrentLocomotionState = LocomotionState.WallCling;

            isKnockBack = false; //壁につくとノックバック終わり
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

    private void UpdateActionState() //行動状態更新
    {
        if (playerHp <= 0)
        {
            CurrentActionState = ActionState.Dead;
            Debug.Log("死亡");
        }
        else if (isAttack)
        {
            CurrentActionState = ActionState.Attacking;
            Debug.Log("アタック");
        }
        else
        {
            CurrentActionState = ActionState.Nomal;
        }
    }

    private void UpdateAnimation() //アニメーションの更新
    {
        //死亡
        if (CurrentActionState == ActionState.Dead)
        {
            anim.SetBool("Death", true);
        }

        //地上での走り
        if (moveInput.x != 0 && CurrentLocomotionState == LocomotionState.Grounded)
        {
            transform.localScale = new Vector3(moveInput.x, 1, 1);
            anim.SetBool("Run", true);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            anim.SetBool("Run", false);
            anim.SetBool("WallSlide", false);
        }

        //壁ずり、壁待機、上下移動
        if (isAtLeftLedge || isAtRightLedge)
        {
            transform.localScale = new Vector3(-wallDirection, 1, 1);
            anim.SetBool("WallSlide", true);
        }
        else if (moveInput.y == 0 && CurrentLocomotionState == LocomotionState.WallCling)
        {
            transform.localScale = new Vector3(-wallDirection, 1, 1);
            anim.SetBool("WallSlide", false);
            anim.SetBool("Wall", true);
            anim.SetBool("WallRun", false);
        }
        else if (moveInput.y != 0 && CurrentLocomotionState == LocomotionState.WallCling)
        {
            transform.localScale = new Vector3(-wallDirection, moveInput.y, 1);
            anim.SetBool("WallRun", true);
        }
        else
        {
            anim.SetBool("WallRun", false);
            anim.SetBool("Wall", false);
        }

        //ジャンプ
        if (rb.linearVelocityY > 0 && CurrentLocomotionState == LocomotionState.Airborne)
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Fall", false);
            anim.SetBool("WallSlide", false);
        }
        else if (rb.linearVelocityY <= 0 && CurrentLocomotionState == LocomotionState.Airborne && (!isAtLeftLedge && !isAtRightLedge))
        {
            //transform.localScale = new Vector3(moveInput.x, 1, 1);
            anim.SetBool("Fall", true);
            //Debug.Log("Fall");
        }
        else
        {
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", false);
        }

        //急落下
        if (isFastFalling && CurrentLocomotionState == LocomotionState.Airborne)
        {
            anim.SetBool("FastFall", true);
        }
        else
        {
            anim.SetBool("FastFall", false);
        }

        //アタック
        if (CurrentActionState == ActionState.Attacking)
        {
            anim.SetBool("Attack1", true);
        }
        else
        {
            anim.SetBool("Attack1", false);
        }

        //アタックを受けた
        if (CurrentActionState == ActionState.Hurt)
        {
            anim.SetBool("Hit", true);
        }
        else if (CurrentLocomotionState == LocomotionState.Grounded && CurrentLocomotionState == LocomotionState.WallCling)
        {
            anim.SetBool("Hit", false);
        }
    }

    private void Attack()
    {
        if (CurrentActionState != ActionState.Attacking)
        {
            return;
        }

        if (hasAttacked)
        {
            return;
        }

        hasAttacked = true;

        Collider2D hit = Physics2D.OverlapCircle((Vector2)transform.position+attackPosition, attackRange, enemyLayer);

        if (hit != null)
        {
            Damageable target = hit.GetComponentInParent<Damageable>();
            //target.TakeDamage(playerAttackDamage, playerGiveKnockBack, playerAttackInvincibleTime); //エネミーを置いていないのでエラーが出てしまう。一旦コメントアウトしてます。
            Debug.Log(playerAttackDamage);
        }

        isAttack = false;
    }

    private void OnDrawGizmosSelected()
     { Gizmos.DrawWireSphere( (Vector2)transform.position+attackPosition, attackRange ); }
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

            if (jumpReleased && !isFastFalling && rb.linearVelocityY > 0) //落下速度の調節
            {
                rb.AddForce(Vector2.down * fallPower, ForceMode2D.Impulse);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -fastFallSpeed));

                Debug.Log("落下調節");

                jumpReleased = false;
            }
        }
        else
        {
            jumpReleased = false;
        }
    }

    private void Jump() //ジャンプ
    {
        if (canGroundJump || CurrentLocomotionState == LocomotionState.Airborne)
        {
            if (jumpPressed)
            {
                if (canGroundJump)
                {
                    //Debug.Log("地上ジャンプ");
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);

                    jumpHoldTimer = 0f;
                    jumpPressed = false;
                }

                else if (!canGroundJump && CurrentLocomotionState == LocomotionState.Airborne)
                {
                    if (hasUsedAirJump == false && !wallKickRange) //二回目のジャンプ
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, airJumpPower);

                        jumpHoldTimer = 0f;
                        jumpPressed = false;
                        hasUsedAirJump = true;
                        canWallKick = false;

                        ledgeJumpTimer = 0f; //
                    }
                    else
                    {
                        jumpPressed = false;
                        //canWallKick = false;
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
            canWallKick = false;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        if (isRightTouchingWall || isAtRightLedge) //右の壁なら1
        {
            wallDirection = 1;
        }
        if (isLeftTouchingWall || isAtLeftLedge) //左の壁なら-1
        {
            wallDirection = -1;
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
        if (CurrentLocomotionState == LocomotionState.WallCling && moveInput.x == -wallDirection) //壁から離れる
        {
            float velocityX = moveInput.x * wallDetachForce;
            rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y * detachWallMultiplier);
        }

        if ((isWallKick && moveInput.y >= 0) || canWallKick) //壁キック
        {
            Debug.Log("壁キック");
            float velocityX = -wallDirection * wallDetachForce;
            rb.linearVelocity = new Vector2(velocityX, wallKickForce);

            isWallKick = false;
            canWallKick = false;
        }
        else if (isWallKick && moveInput.y < 0)
        {
            isWallKick = false;
        }
    }

    private void StartLedgeClimb() //ClimbongLedgeへ状態変化
    {
        if (CurrentLocomotionState == LocomotionState.Airborne && (isAtLeftLedge||isAtRightLedge) && moveInput.y > 0)
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
 
            if (ledgeClimbDuration < ledgeJumpUpTime) //ある一定時間まで上方向に上昇
            {
                rb.linearVelocity = new Vector2(0f, ledgeClimbUpSpeed);
            }
            else if (ledgeJumpUpTime <= ledgeClimbDuration && ledgeClimbDuration < ledgeJumpHorizontalTime)
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

    public void OnDamaged(float damage, Vector2 knockBack)
    {
        if (CurrentActionState == ActionState.Dead)
        {
            return;
        }

        playerHp -= damage;

        if (playerHp > 0)
        {
            rb.linearVelocity = knockBack;

            isKnockBack = true;
        }
    }
}
