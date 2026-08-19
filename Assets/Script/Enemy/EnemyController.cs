using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyData dataofenemy;
    [SerializeField] Transform playertransform;
    private bool isInSearchRange;
    private bool isInChaseRange;    private float distanceX;
    void Start()
    {
        isInSearchRange = false;
        isInChaseRange = false;
    }
    void Update()
    {
        //索敵範囲内かの判定
        CheckDistance();
        if(isInChaseRange)
        {
            ChasePlayer();
        }
    }
    void CheckDistance()
    {
        if(!isInSearchRange)
        {
            if(Vector2.Distance(playertransform.position,transform.position) < dataofenemy.enemySearchRange)
            {
                isInSearchRange = true;
                isInChaseRange = true;
            }
        }
        else if(Vector2.Distance(playertransform.position,transform.position) < dataofenemy.enemySearchRange)
        {
            isInSearchRange = false;
            isInChaseRange = false;
        }
    }
    void ChasePlayer()
    {
        
    }
}