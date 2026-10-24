using UnityEngine;

public class NonInteractiveObject : MonoBehaviour
{
    [Header("AutoScroll")]
    [SerializeField] private bool isAutoScroll;
    [SerializeField] private float autoScrollSpeed;

    [Header("FollowingScroll")]
    [SerializeField] private bool isfollowingPlayer;
    [SerializeField] private bool isFixedYAxis;
    [SerializeField] private float scrollSpeed;

    [Header("Appearance")]
    [SerializeField] private Color tint = Color.white;
    [SerializeField, Range(0f,1f)] private float opacity;

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
        if (isAutoScroll)
        {
            transform.position -= new Vector3(autoScrollSpeed, 0f, 0f);
        }
    }

    void ScrollSpeed() //視差
    {
        if (isAutoScroll || !isfollowingPlayer)
        {
            return;
        }

        if (!isFixedYAxis)
        {
            Vector3 cameraMovement = mainCamera.position - startCameraPosition;

            transform.position = cameraMovement * scrollSpeed + startObjectPosition;
        }

        else
        {
            Vector3 cameraMovement = mainCamera.position - startCameraPosition;

            transform.position = new Vector3(cameraMovement.x * scrollSpeed + startObjectPosition.x, transform.position.y, cameraMovement.z * scrollSpeed + startObjectPosition.z);
        }
    }
}
