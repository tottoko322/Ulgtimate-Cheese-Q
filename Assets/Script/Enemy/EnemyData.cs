using UnityEngine;

public class EnemyData : MonoBehaviour
{
    //能力値
    [Header("能力値。基本的に固定値")]
    public float enemyHp;
    public float enemySpeed;
    public float enemyAttack;
    public Vector2 enemyGiveKnockBack;
    public float enemyAttackInvincibleTime;
    [Header("行動決定のための範囲設定")]
    public float enemySearchRange;
    public float enemyChaseRange;
    public float enemyAttackRange;
    [Header("攻撃の当たり判定")]
    public float enemyHitRange;
    public float enemyHitAngle;
    public float enemyHitTime;
    [Header("クールタイム")]
    public float enemyCooltime;

    //アニメーション設定
    [Header("アニメーション時間をそのまま入れる")]
    public float enemyDetectAnimationTime;
    public float enemyAttackAnimationTime;
    public float enemyHurtAnimationTime;
    public float enemyDeadAnimationTime;
    [Header("攻撃アニメーション時の力の調整")]
    public Vector2[] enemyAttackActionForce;
    public float[] enemyAttackActionInterval;


    //public float enemyFixedTimeAttack;
    //public float enemyFixedTimeHurt;
    //public float enemyNondamageTime;
    
    /*public float enemyChangeMoveSpritesInterval;
    public float enemyChangeAttackSpritesInterval;
    public float enemyChangeStaySpritesInterval;
    public float enemyChangeHurtSpritesInterval;
    public float enemyChangeDeadSpritesInterval;
    public Sprite[] enemyMoveAnimationSprites;
    public Sprite[] enemyAttackAnimationSprites;
    public Sprite[] enemyStayAnimationSprites;
    public Sprite[] enemyHurtAnimationSprites;
    public Sprite[] enemyDeadAnimationSprites;*/
}