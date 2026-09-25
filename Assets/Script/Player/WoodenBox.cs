using UnityEngine;

public class WoodenBox : MonoBehaviour
{
    [SerializeField] private float Durability;
    public bool canDestroy;
    public bool canPush;

    public BoxCollider2D col;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        CanPush();
    }

    void FixedUpdate()
    {
        
    }

    void CanPush() //プレイヤーとの衝突の有無
    {
        if (canPush)
        {
            col.enabled = true;
        }
        else
        {
            col.enabled = false;
            rb.gravityScale = 0f;
        }
    }
}
