using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyData dataofenemy;
    [SerializeField] Transform playertransform;
    private bool isInSearchRange;
    private bool isInChaseRange;
    private bool isWalking;
    private float distanceX;
    private float distanceY;
    private int viewMoveSpriteNumber;
    private int viewStaySpriteNumber;
    private SpriteRenderer sr;
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
        if(isInChaseRange)
        {
            ChasePlayer();
        }
    }
    void CheckDistance()
    {
        //索敵処理
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
        else if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
        {
            isInSearchRange = false;
            isInChaseRange = false;
        }
        //追跡処理及びそのアニメーション処理
        ChasePlayer();
        ChangeSprite();
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
        if(isWalking)
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
        else if(!isWalking)
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