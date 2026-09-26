using UnityEngine;

public class NonInteractiveObject : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float zDistance;

    private Transform mainCamera;

    private Vector3 startObjectPosition;
    private Vector3 startCameraPosition;

    void Start()
    {
        mainCamera = Camera.main.transform;

        startObjectPosition = transform.position;
        startCameraPosition = mainCamera.position;
    }

    void LateUpdate()
    {
        ScrollSpeed();
    }

    void ScrollSpeed() //視差
    {
        Vector3 cameraMovement = mainCamera.position - startCameraPosition;

        transform.position = cameraMovement * scrollSpeed + startObjectPosition;
    }
}
