using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform Player;

    [Header("Look Ahead")]
    [SerializeField] private float horizontalLookAhead; //x方向の先読み
    [SerializeField] private float verticalLookAhead; //y方向の先読み

    [Header("Camera Movement")]
    [SerializeField] private float horizontalSmoothTime; //横方向のカメラの追尾する時間(値が小さいと早く追尾)
    [SerializeField] private float verticalSmoothTime; //縦方向のカメラの追尾する時間

    private Vector3 cameraPosition;
    private Vector3 lookAhead;
    private float horizontalVelocity;
    private float verticalVelocity;

    [SerializeField] private InputActionAsset inputActions; //キーコンフィグ
    private InputActionMap keyControl;

    private InputAction moveRightCamera;
    private InputAction moveLeftCamera;
    private InputAction moveUpCamera;
    private InputAction moveDownCamera;

    void Start()
    {
        keyControl = inputActions.FindActionMap("KeyControl", true);
        moveRightCamera = keyControl.FindAction("MoveRight", true);
        moveLeftCamera = keyControl.FindAction("MoveLeft", true);
        moveUpCamera = keyControl.FindAction("WallUp", true);
        moveDownCamera = keyControl.FindAction("WallDown", true);
    }
    
    void Update()
    {
        CameraReadInput();
    }

    void LateUpdate()
    {
        CameraPosition();
    }

    private void CameraReadInput() //WASDキー
    {
        lookAhead = Vector3.zero;

        if (moveRightCamera.IsPressed())
        {
            lookAhead.x += horizontalLookAhead;
        }
        if (moveLeftCamera.IsPressed())
        {
            lookAhead.x -= horizontalLookAhead;
        }

        if (moveUpCamera.IsPressed())
        {
            lookAhead.y += verticalLookAhead;
        }
        if (moveDownCamera.IsPressed())
        {
            lookAhead.y -= verticalLookAhead;
        }
    }

    private void CameraPosition()
    {
        cameraPosition = Player.position + lookAhead; //カメラの位置＝プレイヤーの位置＋先読み

        float xPosition = Mathf.SmoothDamp(transform.position.x, cameraPosition.x, ref horizontalVelocity, horizontalSmoothTime);
        float yPosition = Mathf.SmoothDamp(transform.position.y, cameraPosition.y+1.5f, ref verticalVelocity, verticalSmoothTime);

        transform.position = new Vector3(xPosition, yPosition, -10f);
        //transform.position = Vector3.SmoothDamp(transform.position, cameraPosition + new Vector3(0f, 1f, -10f), ref velocity, cameraSmoothTime);
        //プレイヤーが画面中央ちょい下

        float screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y; 
        float screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        //現在の画面端

        float verticalMargin = 1f; //画面の枠とプレイヤーの距離

        if (Player.position.y + verticalMargin > screenTop)
        {
            float difference = Player.position.y - (screenTop - verticalMargin); //difference＝プレイヤーが来てほしい位置
            transform.position += new Vector3(0f, difference, 0f);
        }

        if (Player.position.y < screenBottom + verticalMargin)
        {
            float difference = (screenBottom + verticalMargin) - Player.position.y;
            transform.position -= new Vector3(0f, difference, 0f);
        }
    }
}
