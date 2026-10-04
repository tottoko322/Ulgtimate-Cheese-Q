using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    //状態保存用
    private enum EnemyStatusBox
    {
        Stay,
        Detect,
        Chase,
        Attack,
        Hurt,
        Dead
    }
    [SerializeField] private EnemyStatusBox EnemyCurrentStatus;
    [SerializeField] private EnemyStatusBox EnemyLatestStatu;
    private bool canChangeStatus;

    //Data,Component取得
    [Header("Component取得")]
    [SerializeField] EnemyData dataofenemy;
    [SerializeField] Damageable damagesystem;
    [SerializeField] Transform playertransform;
    [SerializeField] private Rigidbody2D enemyrb;
    private SpriteRenderer sr;
    [SerializeField] private Animator enemyanim;

    //索敵用
    private float distanceX;
    private float distanceY;
    private float passTimeAfterDetect;

    //移動用
    

    //アニメーション用
    /*private int viewMoveSpriteNumber;
    private int viewAttackSpriteNumber;
    private int viewStaySpriteNumber;
    private int viewHurtSpriteNumber;
    private int viewDeadSpriteNumber;
    private float viewTimeMoveSprite;
    private float viewTimeAttackSprite;
    private float viewTimeStaySprite;
    private float viewTimeHurtSprite;
    private float viewTimeDeadSprite;*/
    private int attackActionPhase;
    private float attackActionTime;
    

    //攻撃用
    private bool isInCoolTime;
    private float passTimeAfterAttackFixed;
    private float passTimeAfterAttackCool;
    public bool willHitEnemyAttack;
    private float passFrameAfterAttackHit;

    //被弾、死亡用
    public bool isDamaged;
    //private bool canBeDamaged;
    [SerializeField] private float damage;
    private Vector2 knockBack;
    [SerializeField] private float currentHP;
    private float passTimeAfterHurtFixed;
    //private float passTimeAfterHurtNonDamage;
    //private bool canBeRemoved;
    private bool isInKnockBack;
    private float passTimeAfterDead;

    //処理
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        EnemyCurrentStatus = EnemyStatusBox.Stay;
        EnemyLatestStatu = EnemyStatusBox.Stay;
        //sr.sprite = dataofenemy.enemyStayAnimationSprites[0];
        canChangeStatus = true;
        isInCoolTime = false;
        isDamaged = false;
        //canBeDamaged = true;
        //canBeRemoved = false;
        isInKnockBack = false;
        currentHP = dataofenemy.enemyHp;
        willHitEnemyAttack = false;
    }
    void Update()
    {
        if(canChangeStatus)
        {
            CheckDistance();
            BeforeDead();
        }
        Dead();
        CheckDamage();
        CheckTime();
        ControlAnimation();
        Damage();
        Attack();
        //Dead();
    }
    void FixedUpdate()
    {
        ChasePlayer();
        AttackAction();
        KnockBack();
    }

    //関数
    void CheckDistance()//Stay,Detect,Chase,Attackへの変更
    {
        distanceX = playertransform.position.x - transform.position.x;
        distanceY = playertransform.position.y - transform.position.y;
        //索敵範囲内か
        if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
        {
            if(EnemyCurrentStatus == EnemyStatusBox.Stay)
            {
                passTimeAfterDetect = 0f;
                EnemyCurrentStatus = EnemyStatusBox.Detect;
                canChangeStatus = false;
            }
            else
            {
                EnemyCurrentStatus = EnemyStatusBox.Chase;
                canChangeStatus = true;
            }
        }
        if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemyAttackRange*dataofenemy.enemyAttackRange)
        {
            if(!isInCoolTime)
            {
                EnemyCurrentStatus = EnemyStatusBox.Attack;
                isInCoolTime = true;
                canChangeStatus = false;
                passTimeAfterAttackFixed = 0f;
                passTimeAfterAttackCool = 0f;
                passTimeAfterAttackHit = 0f;
                /*viewAttackSpriteNumber = 0;
                viewTimeAttackSprite = 0f;
                attackActionPhase = -1;//初回のアクションを起こすため
                attackActionTime = 0f;*/
            }
        }
        if(distanceX*distanceX + distanceY*distanceY > dataofenemy.enemyChaseRange*dataofenemy.enemyChaseRange)//追跡範囲内か
        {
            EnemyCurrentStatus = EnemyStatusBox.Stay;
            canChangeStatus = true;
        }
    }
    void CheckTime()
    {
        if(EnemyCurrentStatus == EnemyStatusBox.Detect)
        {
            passTimeAfterDetect += Time.deltaTime;
            if(passTimeAfterDetect >= dataofenemy.enemyDetectAnimationTime)
            {
                canChangeStatus = true;
            }
        }
        if(isInCoolTime)//硬直やクールタイムがアニメーションの表示時間より短い場合のでバックログを追加予定
        {
            passTimeAfterAttackCool += Time.deltaTime;
            if(passTimeAfterAttackCool >= dataofenemy.enemyCooltime)
            {
                isInCoolTime = false;
            }
        }
        if(EnemyCurrentStatus == EnemyStatusBox.Attack)
        {
            passTimeAfterAttackFixed += Time.deltaTime;
            if(passTimeAfterAttackFixed >= dataofenemy.enemyAttackAnimationTime)
            {
                canChangeStatus = true;
            }
        }
        if(EnemyCurrentStatus == EnemyStatusBox.Hurt)
        {
            passTimeAfterHurtFixed += Time.deltaTime;
            if(passTimeAfterHurtFixed >= dataofenemy.enemyHurtAnimationTime)
            {
                canChangeStatus = true;
            }
        }
        /*if(!canBeDamaged)
        {
            if(passTimeAfterHurtNonDamage >= dataofenemy.enemyNondamageTime)
            {
                canBeDamaged = true;
            }
        }*/
    }
    void CheckDamage()
    {
        if(isDamaged) //&& canBeDamaged)
        {
            EnemyCurrentStatus = EnemyStatusBox.Hurt;
            //何かでdamageの値を取得する(damagesystemからの取得を想定)
            currentHP -= damage;
            isDamaged = false;
            //canBeDamaged = false;
            isInKnockBack = true;
            passTimeAfterHurtFixed = 0f;
            //passTimeAfterHurtNonDamage = 0f;
            /*viewHurtSpriteNumber = 0;
            viewTimeHurtSprite = 0f;*/
            canChangeStatus = false;
        }
    }
    void ControlAnimation()
    {
        if(EnemyLatestStatu != EnemyCurrentStatus)
        {
            string enemyAnimationTrigger = EnemyCurrentStatus switch
            {
                EnemyStatusBox.Stay => "StayTrigger",
                EnemyStatusBox.Detect => "DetectTrigger",
                EnemyStatusBox.Chase => "ChaseTrigger",
                EnemyStatusBox.Attack => "AttackTrigger",
                EnemyStatusBox.Hurt => "HurtTrigger",
                EnemyStatusBox.Dead => "DeadTrigger",
                _ => "ChaseStatus"//趣味。バグったときは動いていてほしい()
            };
            enemyanim.SetTrigger(enemyAnimationTrigger);
            EnemyLatestStatu = EnemyCurrentStatus;
        }
    }
    void ChasePlayer()
    {
        if(EnemyCurrentStatus == EnemyStatusBox.Chase)
        {
            if(distanceX < 0f)
            {
                enemyrb.linearVelocityX = -dataofenemy.enemySpeed;//左向き
                if(transform.localScale.x > 0)
                {
                    transform.localScale *= -1;
                }
            }
            else if(distanceX > 0f)
            {
                enemyrb.linearVelocityX = dataofenemy.enemySpeed;//右向き
                if(transform.localScale.x < 0)
                {
                    transform.localScale *= -1;
                }
            }
        }
        else
        {
            enemyrb.linearVelocityX = 0f;
        }
    }
    void AttackAction()
    {

        if(EnemyCurrentStatus == EnemyStatusBox.Attack && attackActionPhase < dataofenemy.enemyAttackActionInterval.Length - 1)
        {
            if(attackActionPhase == -1)
            {
                attackActionPhase ++ ;
                enemyrb.AddForce(dataofenemy.enemyAttackActionForce[0],ForceMode2D.Impulse);
            }
            attackActionTime += Time.fixedDeltaTime;
            if(attackActionTime >= dataofenemy.enemyAttackActionInterval[attackActionPhase])
            {
                attackActionPhase ++ ;
                enemyrb.AddForce(dataofenemy.enemyAttackActionForce[attackActionPhase],ForceMode2D.Impulse);
                attackActionTime = 0f;
            }
        }
    }
    void Attack()
    {
        if(EnemyCurrentStatus == EnemyStatusBox.Attack)
        {
            passTimeAfterAttackHit += Time.deltaTime;
            if(passTimeAfterAttackHit == dataofenemy.enemyHitFrame)
            {
                willHitEnemyAttack = true;
            }
        }
    }
    void KnockBack()
    {
        if(isInKnockBack)
        {
            enemyrb.AddForce(knockBack,ForceMode2D.Impulse);
            isInKnockBack = false;
        }
    }
    void BeforeDead()
    {
        if(currentHP <= 0)
        {
            EnemyCurrentStatus = EnemyStatusBox.Dead;
            canChangeStatus = false;
        }
    }
    void Dead()
    {
        if(EnemyCurrentStatus == EnemyStatusBox.Dead)
        {
            while (passTimeAfterDead >= dataofenemy.enemyDeadAnimationTime)
            {
                passTimeAfterDead += Time.deltaTime;
            }
            Debug.Log("enemyDie");
            gameObject.SetActive(false);
        }
    }

    //debug用
    void Damage()
    {
        if(Keyboard.current.mKey.isPressed)
        {
            isDamaged = true;
        }
    }

    //産廃
    /*void ChangeSprite()
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
        else if(EnemyCurrentStatus == EnemyStatusBox.Hurt)//被弾アニメーション
        {
            if(viewHurtSpriteNumber < dataofenemy.enemyHurtAnimationSprites.Length - 1)
            {
                if(viewTimeHurtSprite >= dataofenemy.enemyChangeHurtSpritesInterval)
                {
                    viewHurtSpriteNumber ++;
                    viewTimeHurtSprite = 0f;
                }
                else
                {
                    viewTimeHurtSprite += Time.deltaTime;
                }
                sr.sprite = dataofenemy.enemyHurtAnimationSprites[viewHurtSpriteNumber];
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
        else if(EnemyCurrentStatus == EnemyStatusBox.Dead)//被弾アニメーション
        {
            if(viewDeadSpriteNumber < dataofenemy.enemyDeadAnimationSprites.Length - 1)
            {
                if(viewTimeDeadSprite >= dataofenemy.enemyChangeDeadSpritesInterval)
                {
                    viewDeadSpriteNumber ++;
                    viewTimeDeadSprite = 0f;
                }
                else
                {
                    viewTimeDeadSprite += Time.deltaTime;
                }
                sr.sprite = dataofenemy.enemyDeadAnimationSprites[viewDeadSpriteNumber];
            }
            else if(viewDeadSpriteNumber == dataofenemy.enemyDeadAnimationSprites.Length - 1)
            {
                canBeRemoved = true;
            }
        }
    }*/
}