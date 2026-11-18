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
    //[SerializeField] private bool isFixedYAxis; //プレイヤーを追従する際、Y軸を固定するか否か
    //[SerializeField] private float fixedYAxisRange; //isFixedYAxisがtrueのときY軸固定に範囲を持たせる
    [SerializeField, Range(0f, 1f)] private float parallaxRatioX; //X軸について、カメラ移動に対する背景の移動割合
    [SerializeField, Range(0f, 1f)] private float parallaxRatioY; //Y軸について、カメラ移動に対する背景の移動割合
    

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
        Vector3 autoMove = Vector3.zero;
        if (isSideAutoScroll)
        {
            autoMove.x -= sideAutoScrollSpeed;
        }

        if (isVerticalAutoScroll)
        {
            autoMove.y += verticalAutoScrollSpeed;
        }
        transform.position += autoMove * Time.deltaTime;
    }

    void ScrollSpeed() //視差
    {
        if (isSideAutoScroll || isVerticalAutoScroll || !isfollowingPlayer)
        {
            return;
        }

        Vector3 cameraMovement = mainCamera.position - startCameraPosition;
        //Vector3 targetPosition = cameraMovement + startObjectPosition;

        Vector3 targetPosition = startObjectPosition + new Vector3(cameraMovement.x * parallaxRatioX, cameraMovement.y * parallaxRatioY, 0f);
        
        //if (isFixedYAxis)
        //{
        //    targetPosition.y = Mathf.Clamp(targetPosition.y,startObjectPosition.y - fixedYAxisRange,startObjectPosition.y + fixedYAxisRange);
        //}

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followingSpeed * Time.deltaTime);
    }
}
