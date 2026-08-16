using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpPower = 10f; 
    [SerializeField] private LayerMask Ground;

    enum JumpState
    {
        ground,
        jump,
        doubleJump
    }
    private JumpState jumpState = JumpState.ground;
    private Rigidbody2D rb;
    private bool isGrounded;

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
        Move();
    }
    private void Move()
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
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            break;
            case 5:
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            break;
            case 6:
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            break;
        }
    }

    private void Jump()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y -0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, Ground);
        if (hit.collider != null && rb.linearVelocity.y <= 0)
        {
            jumpState = JumpState.ground;
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            switch(jumpState)
            {
                case JumpState.ground:
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpState = JumpState.jump;
                break;

                case JumpState.jump:
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpState = JumpState.doubleJump;
                break;

                case JumpState.doubleJump:
                break;
            }
        }
    }
    void OnDrawGizmos()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y -0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, new Vector2(0, -0.1f));
    }
}
