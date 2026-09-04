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
    [SerializeField] Damageable damagesystem;
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
    //被弾、死亡用
    public bool isDamaged;
    private float damage;
    private float currentHP;
    //処理
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = dataofenemy.enemyStayAnimationSprites[0];
        isInCoolTime = false;
    }
    void Update()
    {
        Damaged();
        //索敵範囲内かの判定
        CheckDistance();
        //追跡処理
        ChasePlayer();
        //攻撃処理
        //被弾、死亡処理
        //アニメーション処理
        ChangeSprite();
        CheckTime();
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
        else if(distanceX*distanceX + distanceY*distanceY > dataofenemy.enemyChaseRange*dataofenemy.enemyChaseRange)//追跡範囲内か
        {
            EnemyCurrentStatus = EnemyStatusBox.Stay;
        }
        if(EnemyCurrentStatus == EnemyStatusBox.Chase)
        {
            if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemyAttackRange*dataofenemy.enemyAttackRange)
            {
                if(!isInCoolTime)
                {
                    EnemyCurrentStatus = EnemyStatusBox.Attack;
                    isInCoolTime = true;
                }
                else
                {
                    EnemyCurrentStatus = EnemyStatusBox.Stay;
                }
            }
        }
    }
    void CheckTime()
    {
        if(isInCoolTime)
        {
            passTimeAfterAttack += Time.deltaTime;
            if(EnemyCurrentStatus == EnemyStatusBox.Attack && passTimeAfterAttack > dataofenemy.enemyFixedTime)
            {
                EnemyCurrentStatus = EnemyStatusBox.Chase;
            }
            if(passTimeAfterAttack >= dataofenemy.enemyCooltime)
            {
                isInCoolTime = false;
                passTimeAfterAttack = 0f;
                viewAttackSpriteNumber = 0;
            }
        }
    }
    void ChasePlayer()
    {
        if(EnemyCurrentStatus == EnemyStatusBox.Chase)
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
    }
    void Damaged()
    {
        if(isDamaged)
        {
            EnemyCurrentStatus = EnemyStatusBox.Hurt;
            //何かでdamageの値を取得する(damagesystemからの取得を想定)
            currentHP -= damage;
            isDamaged = false;
            if(currentHP <= 0f)
            {
                EnemyCurrentStatus = EnemyStatusBox.Dead;
            }
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
        else if(EnemyCurrentStatus == EnemyStatusBox.Attack)//攻撃アニメーション
        {
            if(viewAttackSpriteNumber < dataofenemy.enemyAttackAnimationSprites.Length - 1)
            {
                if(viewTimeAttackSprite >= dataofenemy.enemyChangeAttackSpritesInterval)
                {
                    viewAttackSpriteNumber ++;
                    viewTimeAttackSprite = 0f;
                }
                else
                {
                    viewTimeAttackSprite += Time.deltaTime;
                }
                sr.sprite = dataofenemy.enemyAttackAnimationSprites[viewAttackSpriteNumber];
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