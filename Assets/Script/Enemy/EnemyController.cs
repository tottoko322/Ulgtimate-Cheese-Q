using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //状態保存用
    private enum EnemyStatusBox
    {
        Stay,
        Chase,
        Attack,
        Hurt,
        Dead
    }
    private EnemyStatusBox EnemyCurrentStatus = EnemyStatusBox.Stay;
    //Data取得
    [SerializeField] EnemyData dataofenemy;
    //索敵用
    private float distanceX;
    private float distanceY;
    //移動用
    [SerializeField] Transform playertransform;
    //アニメーション用
    private int viewMoveSpriteNumber;
    private int viewAttackSpriteNumber;
    private int viewStaySpriteNumber;
    private float viewTimeMoveSprite;
    private float viewTimeAttackSprite;
    private float viewTimeStaySprite;
    private SpriteRenderer sr;
    //攻撃用
    private bool isInCoolTime;
    private float passTimeAfterAttack;
    private float passFixedTime;
    //処理
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = dataofenemy.enemyStayAnimationSprites[0];
    }
    void Update()
    {
        //索敵範囲内かの判定
        CheckDistance();
        //追跡処理
        if(EnemyCurrentStatus == EnemyStatusBox.Chase)
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
        if(EnemyCurrentStatus == EnemyStatusBox.Stay)//索敵範囲内か
        {
            if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
            {
                EnemyCurrentStatus = EnemyStatusBox.Chase;
            }
        }
        else if(EnemyCurrentStatus == EnemyStatusBox.Chase && !isInCoolTime)
        {
            if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemyCooltime*dataofenemy.enemyCooltime)
            {
                EnemyCurrentStatus = EnemyStatusBox.Attack;
                isInCoolTime = true;
            }
        }
        else if(distanceX*distanceX + distanceY*distanceY > dataofenemy.enemyChaseRange*dataofenemy.enemyChaseRange)//追跡範囲内か
        {
            EnemyCurrentStatus = EnemyStatusBox.Stay;
        }
    }
    void CheckTime()
    {
        if(EnemyCurrentStatus == EnemyStatusBox.Attack)
        {
            
            if(passFixedTime < dataofenemy.enemyFixedTime)
            {
                passFixedTime += Time.deltaTime;
            }
            else
            {
                EnemyCurrentStatus = EnemyStatusBox.Chase;
                passFixedTime = 0f;
            }
        }
        if(isInCoolTime)
        {
            passTimeAfterAttack += Time.deltaTime;
            if(passTimeAfterAttack >= dataofenemy.enemyCooltime)
            {
                isInCoolTime = false;
            }
        }
    }
    void ChasePlayer()
    {
        if(distanceX < 0f)
        {
            transform.Translate(Vector3.left*dataofenemy.enemySpeed*Time.deltaTime);
        }
        else if(distanceX > 0f)
        {
            transform.Translate(Vector3.right*dataofenemy.enemySpeed*Time.deltaTime);
        }
    }
    void ChangeSprite()
    {
        if(EnemyCurrentStatus == EnemyStatusBox.Chase)//追跡アニメーション
        {
            if(viewTimeMoveSprite >= dataofenemy.enemyChangeMoveSpritesInterval)
            {
                if(viewMoveSpriteNumber == dataofenemy.enemyMoveAnimationSprites.Length - 1)
                {
                    viewMoveSpriteNumber = 0;
                }
                else
                {
                    viewMoveSpriteNumber ++;
                }
                sr.sprite = dataofenemy.enemyMoveAnimationSprites[viewMoveSpriteNumber];
                if(distanceX < 0f)
                {
                    sr.flipX = true;//右向き素材想定。左向きのときfalse
                }
                else
                {
                    sr.flipX = false;//右向き素材前提
                }
                viewTimeMoveSprite = 0f;
            }
            else
            {
                viewTimeMoveSprite += Time.deltaTime;
            }
        }
        else if(EnemyCurrentStatus == EnemyStatusBox.Attack)
        {
            if(viewTimeAttackSprite >= dataofenemy.enemyChangeAttackSpritesInterval)
            {
                if(viewAttackSpriteNumber < dataofenemy.enemyAttackAnimationSprites.Length)
                {
                    viewAttackSpriteNumber ++;
                    sr.sprite = dataofenemy.enemyAttackAnimationSprites[viewAttackSpriteNumber];
                    viewTimeAttackSprite = 0f;
                }
            }
            else
            {
                viewTimeAttackSprite += Time.deltaTime;
            }
        }
        else if(EnemyCurrentStatus == EnemyStatusBox.Stay)//待機アニメーション
        {
            if(viewTimeStaySprite >= dataofenemy.enemyChangeStaySpritesInterval)
            {
                if(viewStaySpriteNumber == dataofenemy.enemyStayAnimationSprites.Length - 1)
                {
                    viewStaySpriteNumber = 0;
                }
                else
                {
                    viewStaySpriteNumber ++;
                }
                sr.sprite = dataofenemy.enemyStayAnimationSprites[viewStaySpriteNumber];
                viewTimeStaySprite = 0f;
            }
            else
            {
                viewTimeStaySprite += Time.deltaTime;
            }
        }
    }
}