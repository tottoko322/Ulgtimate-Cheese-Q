using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public float enemyHp;
    public float enemyAttack;
    public Vector3 enemySpeed;//yとzは0
    public float enemySearchRange;
    public float enemyChaseRange;
    public float enemyAttackRange;
    public float enemyCooltime;
    public Sprite[] moveAnimationSprites;
    public Sprite[] attackAnimationSprites;
    public Sprite[] stayAnimationSprites;
}
