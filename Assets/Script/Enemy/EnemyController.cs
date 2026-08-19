using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyData dataofenemy;
    [SerializeField] Transform playertransform;
    private bool isInSearchRange;
    private bool isInChaseRange;
    private float distanceX;
    private float distanceY;

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
            distanceX = playertransform.position.x - transform.position.x;
            distanceY = playertransform.position.y - transform.position.y;
            if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
            {
                isInSearchRange = true;
                isInChaseRange = true;
            }
        }
        else
        {
            distanceX = playertransform.position.x - transform.position.x;
            distanceY = playertransform.position.y - transform.position.y;
            if(distanceX*distanceX + distanceY*distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
            {
                isInSearchRange = false;
                isInChaseRange = false;
            }
        }
    }
    void ChasePlayer()
    {
        
    }
}