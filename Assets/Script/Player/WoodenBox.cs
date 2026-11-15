using UnityEngine;

public class WoodenBox : MonoBehaviour
{
    [SerializeField] private int durability; //耐久値(木箱のHP)
    [SerializeField] private bool canDestroy; //木箱が破壊可能か否か
    [SerializeField] private bool canPush; //木箱が押せるか否か(移動可能か否か)
    [SerializeField] private bool canPassingThrough; //木箱に当たり判定があるか否か

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
        if (canPush) //衝突、移動あり
        {
            col.enabled = true;
        }
        else if (!canPush && !canPassingThrough) //衝突あり
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        else //衝突、移動なし
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
