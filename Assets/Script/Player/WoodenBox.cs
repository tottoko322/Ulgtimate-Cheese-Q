using UnityEngine;

public class WoodenBox : MonoBehaviour
{
    [SerializeField] private int durability;
    [SerializeField] private bool canDestroy;
    [SerializeField] private bool canPush;

    public PolygonCollider2D col;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        CanPush();
    }

    void FixedUpdate()
    {
        BlowAway();
        UpdateDurability();
        DestroyWoodenBox();
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

    void BlowAway() //攻撃を受けると吹き飛ぶ
    {
        
    }

    void UpdateDurability() //耐久値の計算
    {
        
    }

    void DestroyWoodenBox() //木箱を壊す
    {
        if (!canDestroy)
        {
            return;
        }

        else if (canDestroy && durability <= 0f)
        {
            this.gameObject.SetActive(false);
        }
    }
}
