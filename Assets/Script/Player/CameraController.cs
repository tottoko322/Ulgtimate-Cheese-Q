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
    [SerializeField] private float cameraSmoothTime;

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

        transform.position = Vector3.SmoothDamp(transform.position, cameraPosition + new Vector3(0f, 1f, -10f), ref velocity, cameraSmoothTime);
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
