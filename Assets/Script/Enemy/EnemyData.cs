using UnityEngine;

public class EnemyData : MonoBehaviour
{
    //能力値
    [Header("能力値")]
    public float enemyHp;
    public float enemyAttack;
    public Vector2 enemyGiveKnockBack;
    public float enemySpeed;
    public float enemySearchRange;
    public float enemyChaseRange;
    public float enemyAttackRange;
    public float enemyCooltime;
    public float enemyHitTiming;
    //public float enemyFixedTimeAttack;
    //public float enemyFixedTimeHurt;
    //public float enemyNondamageTime;

    //アニメーション設定
    [Header("アニメーション設定/状況把握")]
    public float enemyDetectAnimationTime;
    public float enemyAttackAnimationTime;
    public float enemyHurtAnimationTime;
    public float enemyDeadAnimationTime;
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
    public Vector2[] enemyAttackActionForce;
    public float[] enemyAttackActionInterval;
}