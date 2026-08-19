using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public float enemyHp;
    public float enemyAttack;
    public float enemySpeed;
    public float enemySearchRange;
    public float enemyChaseRange;
    public float enemyAttackRange;
    public float enemyCooltime;
    public float distanceX;
    public float distanceY;
    public Sprite[] moveAnimationSprites;
    public Sprite[] attackAnimationSprites;
}
