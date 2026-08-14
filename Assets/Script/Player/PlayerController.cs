using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpPower = 10f; 

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
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            switch(jumpState)
            {
                case JumpState.ground:
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpState = JumpState.jump;
                break;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpState = JumpState.ground;
        }
    }
}
