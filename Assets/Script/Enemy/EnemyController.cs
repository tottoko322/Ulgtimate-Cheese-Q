using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //Data取得
    [SerializeField] EnemyData dataofenemy;
    //索敵用
    private bool isInSearchRange;
    private bool isInChaseRange;
    private float distanceX;
    private float distanceY;
    //移動用
    [SerializeField] Transform playertransform;
    //アニメーション用
    private int viewMoveSpriteNumber;
    private int viewStaySpriteNumber;
    private SpriteRenderer sr;
    //処理
    void Start()
    {
        isInSearchRange = false;
        isInChaseRange = false;
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = dataofenemy.stayAnimationSprites[0];
    }
    void Update()
    {
        //索敵範囲内かの判定
        CheckDistance();
        //追跡処理
        if(isInChaseRange)//おそらく&&!isAttackRangeにする？今は考慮しないで良い
        {
            ChasePlayer();
        }
        //攻撃処理
        //被弾、死亡処理
        //アニメーション処理
        ChangeSprite(); 
    }
    //関数
    void CheckDistance()
    {
        distanceX = playertransform.position.x - transform.position.x;
        distanceY = playertransform.position.y - transform.position.y;
        if(!isInSearchRange)
        {
            if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
            {
                isInSearchRange = true;
                isInChaseRange = true;
            }
        }
        else if(distanceX*distanceX + distanceY*distanceY > dataofenemy.enemyChaseRange*dataofenemy.enemyChaseRange)
        {
            isInSearchRange = false;
            isInChaseRange = false;
        }
    }
    void ChasePlayer()
    {
        if(distanceX < 0f)
        {
            transform.position -= dataofenemy.enemySpeed*Time.deltaTime;
        }
        else if(distanceX > 0f)
        {
            transform.position += dataofenemy.enemySpeed*Time.deltaTime;
        }
    }
    void ChangeSprite()
    {
        if(isInChaseRange)
        {
            if(viewMoveSpriteNumber == dataofenemy.moveAnimationSprites.Length)
            {
                viewMoveSpriteNumber = 0;
            }
            else
            {
                viewMoveSpriteNumber ++;
            }
            sr.sprite = dataofenemy.moveAnimationSprites[viewMoveSpriteNumber];
            if(distanceX < 0f)
            {
                sr.flipX = true;//右向き素材想定。左向きのときfalse
            }
            else
            {
                sr.flipX = false;//右向き素材前提
            }
        }
        else if(!isInChaseRange)
        {
            if(viewStaySpriteNumber == dataofenemy.stayAnimationSprites.Length)
            {
                viewStaySpriteNumber = 0;
            }
            else
            {
                viewStaySpriteNumber ++;
            }
            sr.sprite = dataofenemy.stayAnimationSprites[viewStaySpriteNumber];
        }
    }
}