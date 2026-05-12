using UnityEngine;
using UnityEngine.InputSystem;

public class TPSCamera : MonoBehaviour
{
    [SerializeField]
    private Transform playerBody;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float mouseSensitivity = 0.1f;

    [SerializeField]
    private float cameraDistance = 5f;

    [SerializeField]
    private float cameraSmoothSpeed = 10f;

    [SerializeField]
    private LayerMask wallLayer;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private Vector3 currentCameraLocalPos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        currentCameraLocalPos = new Vector3(0, 0, -cameraDistance);
    }

    void Update()
    {
        RotateCamera();
        HandleCameraCollision();
    }

    void RotateCamera()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);

        transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleCameraCollision()
    {
        float targetDistance = cameraDistance;

        RaycastHit hit;

        // Pivot位置
        Vector3 pivotPos = transform.position;

        // 後ろ方向
        Vector3 dir = -transform.forward;

        // 壁判定
        if (Physics.SphereCast(
            pivotPos,
            0.3f,
            dir,
            out hit,
            cameraDistance,
            wallLayer))
        {
            targetDistance = hit.distance;
        }

        // 目標ローカル位置
        Vector3 targetLocalPos =
            new Vector3(0, 0, -targetDistance);

        // なめらか移動
        currentCameraLocalPos =
            Vector3.Lerp(
                currentCameraLocalPos,
                targetLocalPos,
                Time.deltaTime * cameraSmoothSpeed);

        cameraTransform.localPosition =
            currentCameraLocalPos;
    }
}