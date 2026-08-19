using Unity.XR.GoogleVr;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyData dataofenemy;
    [SerializeField] Transform playertransform;
    private bool isInSearchRange;
    private bool isInChaseRange;
    void Start()
    {
        isInSearchRange = false;
        isInChaseRange = false;
    }
    void Update()
    {
        //索敵範囲内かの判定
        CheckDistance();
    }
    void CheckDistance()
    {
        if(!isInSearchRange)
        {
            dataofenemy.distanceX = playertransform.position.x - transform.position.x;
            dataofenemy.distanceY = playertransform.position.y - transform.position.y;
            if(dataofenemy.distanceX*dataofenemy.distanceX + dataofenemy.distanceY*dataofenemy.distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
            {
                isInSearchRange = true;
                isInChaseRange = true;
            }
        }
        else
        {
            dataofenemy.distanceX = playertransform.position.x - transform.position.x;
            dataofenemy.distanceY = playertransform.position.y - transform.position.y;
            if(dataofenemy.distanceX*dataofenemy.distanceX + dataofenemy.distanceY*dataofenemy.distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
            {
                isInSearchRange = false;
                isInChaseRange = false;
            }
        }
    }
}