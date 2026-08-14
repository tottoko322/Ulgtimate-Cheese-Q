using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public float enemyHp;
    public float enemyAttack;
    public float enemySpped;
    public float enemyDetect;
    public float enemyArm;
    public float enemyCooltime;
    public bool isInDetect;
    public GameObject player;
    private float distanceX;
    private float distanceY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isInDetect = false;
    }

    // Update is called once per frame
    void Update()
    {
        //索敵範囲内かの判定
        if(!isInDetect)
        {
            distanceX = transform.position.x - player.transform.position.x;
            distanceY = transform.position.y - player.transform.position.y;
            if(distanceX*distanceX + distanceY*distanceY < enemyDetect*enemyDetect)
            {
                isInDetect = true;
            }
        }
    }
}
