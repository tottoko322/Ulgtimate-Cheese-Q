using Unity.XR.GoogleVr;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyData dataofenemy;
    [SerializeField] Transform playertransform;
    // Update is called once per frame
    void Update()
    {
        //索敵範囲内かの判定
        CheckDistance();
    }
    void CheckDistance()
    {
        if(!dataofenemy.isInSearchRange)
        {
            dataofenemy.distanceX = playertransform.position.x - transform.position.x;
            dataofenemy.distanceY = playertransform.position.y - transform.position.y;
            if(dataofenemy.distanceX*dataofenemy.distanceX + dataofenemy.distanceY*dataofenemy.distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
            {
                dataofenemy.isInSearchRange = true;
                dataofenemy.isInChaseRange = true;
            }
        }
        else
        {
            dataofenemy.distanceX = playertransform.position.x - transform.position.x;
            dataofenemy.distanceY = playertransform.position.y - transform.position.y;
            if(dataofenemy.distanceX*dataofenemy.distanceX + dataofenemy.distanceY*dataofenemy.distanceY < dataofenemy.enemySearchRange*dataofenemy.enemySearchRange)
            {
                dataofenemy.isInSearchRange = false;
                dataofenemy.isInChaseRange = false;
            }
        }
    }
}