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

    private Vector3 targetPosition;
    private Vector3 lookOffset;

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

    private void CameraReadInput()
    {
        lookOffset = Vector3.zero;

        if (Keyboard.current.dKey.isPressed)
        {
            lookOffset.x += horizontalLookAhead;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            lookOffset.x -= horizontalLookAhead;
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            lookOffset.y += verticalLookAhead;
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {
            lookOffset.y -= verticalLookAhead;
        }
    }

    private void CameraPosition()
    {

        targetPosition = Player.position + lookOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition + new Vector3(0f, 0f, -10f), cameraMoveSpeed * Time.deltaTime);
    }
}
