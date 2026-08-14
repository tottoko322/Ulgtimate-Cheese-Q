using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpPower = 5f; 

    private Rigidbody2D rb;
    private int playerState = 5;
    private bool isGrounded; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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
        if (Keyboard.current.sKey.isPressed)
        {
            playerState -= 3;
        }

        switch(playerState)
        {
            case 1:
            break;
            case 2:
            break;
            case 3:
            break;
            case 4:
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, -moveSpeed, 25*Time.deltaTime), rb.linearVelocity.y);
            break;
            case 5:
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            break;
            case 6:
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, moveSpeed, 25*Time.deltaTime), rb.linearVelocity.y);
            break;
        }
    }
}
