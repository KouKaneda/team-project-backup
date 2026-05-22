using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("追従するターゲット")]
    public Transform target;

    [Header("カメラの距離")]
    public float distance = 3.0f;

    // ★追加：高さを調整するための変数
    [Header("高さのオフセット")]
    public float heightOffset = 1.0f;

    [Header("回転スピード")]
    public float xSpeed = 60.0f;
    public float ySpeed = 60.0f;

    [Header("上下の回転制限")]
    public float yMinLimit = 0f;
    public float yMaxLimit = 40f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        if (target != null)
        {
            x = transform.eulerAngles.y;
            float currentPitch = transform.eulerAngles.x;
            y = currentPitch > 180f ? currentPitch - 360f : currentPitch;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            if (Time.timeSinceLevelLoad > 0.1f && mouseDelta.sqrMagnitude < 10000f)
            {
                x += mouseDelta.x * xSpeed * 0.01f;
                y -= mouseDelta.y * ySpeed * 0.01f;
            }
        }

        y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
        Quaternion rotation = Quaternion.Euler(y, x, 0);

        // ★計算式を変更：target.position に heightOffset を足した地点を「回転の中心」にします
        Vector3 targetPosition = target.position + new Vector3(0, heightOffset, 0);

        transform.position = targetPosition - (rotation * Vector3.forward * distance);
        transform.rotation = rotation;
    }
}