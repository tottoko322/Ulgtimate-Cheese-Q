using UnityEngine;

public class EnemyData : MonoBehaviour
{
    //能力値
    public float enemyHp{get;private set;}
    public float enemyAttack{get;private set;}
    public float enemySpeed{get;private set;}
    public float enemySearchRange{get;private set;}
    public float enemyChaseRange{get;private set;}
    public float enemyAttackRange{get;private set;}
    public float enemyCooltime{get;private set;}
    public float enemyFixedTime{get;private set;}
    //アニメーション設定
    public float enemyChangeMoveSpritesInterval{get;private set;}
    public float enemyChangeAttackSpritesInterval{get;private set;}
    public float enemyChangeStaySpritesInterval{get;private set;}
    public Sprite[] enemyMoveAnimationSprites{get;private set;}
    public Sprite[] enemyAttackAnimationSprites{get;private set;}
    public Sprite[] enemyStayAnimationSprites{get;private set;}
}
