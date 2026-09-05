using UnityEngine;

public class EnemyData : MonoBehaviour
{
    //能力値
    public float enemyHp;
    public float enemyAttack;
    public float enemySpeed;
    public float enemySearchRange;
    public float enemyChaseRange;
    public float enemyAttackRange;
    public float enemyCooltime;
    public float enemyFixedTimeAttack;
    public float enemyFixedTimeHurt;
    //アニメーション設定
    public float enemyChangeMoveSpritesInterval;
    public float enemyChangeAttackSpritesInterval;
    public float enemyChangeStaySpritesInterval;
    public Sprite[] enemyMoveAnimationSprites;
    public Sprite[] enemyAttackAnimationSprites;
    public Sprite[] enemyStayAnimationSprites;
}
