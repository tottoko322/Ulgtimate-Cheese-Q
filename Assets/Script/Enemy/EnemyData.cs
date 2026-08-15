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
    public bool isInSearchRange;
    public bool isInChaseRange;
    public float distanceX;
    public float distanceY;
    public Sprite[] moveAnimationSprites;
    public Sprite[] attackAnimationSprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isInSearchRange = false;
        isInChaseRange = false;
    }

}
