using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform Player;

    [Header("Look Ahead")]
    [SerializeField] private float horizontalLookAhead;
    [SerializeField] private float verticalLookAhead;

    [Header("Camera Movement")]
    [SerializeField] private float cameraMoveSpeed;

    private Vector3 cameraPosition;
    private Vector3 lookAhead;
    private Vector3 velocity;

    void Start()
    {
        
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

        if (Keyboard.current.dKey.isPressed)
        {
            lookAhead.x += horizontalLookAhead;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            lookAhead.x -= horizontalLookAhead;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            lookAhead.y += verticalLookAhead;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            lookAhead.y -= verticalLookAhead;
        }
    }

    private void CameraPosition()
    {
        cameraPosition = Player.position + lookAhead; //カメラの位置＝プレイヤーの位置＋先読み

        transform.position = Vector3.SmoothDamp(transform.position, cameraPosition + new Vector3(0f, 1f, -10f), ref velocity, cameraMoveSpeed);
        //プレイヤーが画面中央ちょい下
    }
}
