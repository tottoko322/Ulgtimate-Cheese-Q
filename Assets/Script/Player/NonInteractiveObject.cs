using UnityEngine;

public class NonInteractiveObject : MonoBehaviour
{
    [Header("AutoScroll")]
    [SerializeField] private bool isSideAutoScroll; //横スクロールか否か
    [SerializeField] private float sideAutoScrollSpeed; //横スクロールの速度

    [SerializeField] private bool isVerticalAutoScroll; //縦スクロールか否か
    [SerializeField] private float verticalAutoScrollSpeed; //縦スクロールの速度
    
    [Header("FollowingScroll")]
    [SerializeField] private bool isfollowingPlayer; //プレイヤーを追従するか否か
    [SerializeField] private float followingSpeed; //プレイヤーを追従する速度
    [SerializeField] private bool isFixedYAxis; //プレイヤーを追従する際、Y軸を固定するか否か
    [SerializeField] private float fixedYAxisRange; //isFixedYAxisがtrueのときY軸固定に範囲を持たせる
    

    [Header("Appearance")]
    [SerializeField] private Color tint = Color.white; //オブジェクトの色を指定
    [SerializeField, Range(0f,1f)] private float opacity; //オブジェクトの透明度

    private Transform mainCamera;

    private Vector3 startObjectPosition;
    private Vector3 startCameraPosition;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        mainCamera = Camera.main.transform;

        startObjectPosition = transform.position;
        startCameraPosition = mainCamera.position;

        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateApearance();
    }

    void LateUpdate()
    {
        IsAutoScroll();
        ScrollSpeed();
    }

    void UpdateApearance() //色の塗りつぶし具合、透明度
    {
        tint.a = opacity;
        spriteRenderer.color = tint;
    }

    void IsAutoScroll()
    {
        if (isSideAutoScroll)
        {
            transform.position -= new Vector3(sideAutoScrollSpeed, 0f, 0f);
        }

        if (isVerticalAutoScroll)
        {
            transform.position += new Vector3(0f, verticalAutoScrollSpeed, 0f);
        }
    }

    void ScrollSpeed() //視差
    {
        if (isSideAutoScroll || isVerticalAutoScroll || !isfollowingPlayer)
        {
            return;
        }

        Vector3 cameraMovement = mainCamera.position - startCameraPosition;
        Vector3 targetPosition = cameraMovement + startObjectPosition;

        if (isFixedYAxis)
        {
            targetPosition.y = Mathf.Clamp(targetPosition.y,startObjectPosition.y - fixedYAxisRange,startObjectPosition.y + fixedYAxisRange);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followingSpeed * Time.deltaTime);
    }
}
