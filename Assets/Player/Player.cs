using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("ジャンプ設定")]
    public float jumpForce = 5f;       // ジャンプの力
    public LayerMask groundLayer;      // 地面として判定するレイヤー

    [Header("壁走り設定")]
    public float wallRunDuration = 2f; // 壁走りできる時間（秒）
    public float wallJumpForce = 5f;   // 壁蹴りジャンプの力

    private Rigidbody rb;
    private bool isGrounded;           // 地面にいるかどうかのフラグ
    private Transform mainCameraTransform;

    // 壁走り用の変数
    private bool isWallRunning;
    private float wallRunTimer;
    private Vector3 wallNormal;        // 接触している壁の向き（法線）

    // ★追加: 入力があるかどうかを保持する変数
    private bool hasInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        // タイマーの初期化
        wallRunTimer = wallRunDuration;
    }

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;
        bool jumpInput = false;

        // --- 入力の取得 ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame) jumpInput = true;
        }

        // ★変更: 入力の有無を毎フレーム判定して変数に保存
        hasInput = (horizontal != 0f || vertical != 0f);

        // --- 接地判定（Raycast） ---
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, 0.2f, groundLayer);

        // 地面にいる場合、壁走りのタイマーと状態をリセット
        if (isGrounded)
        {
            wallRunTimer = wallRunDuration;
            if (isWallRunning) StopWallRun();
        }

        // --- 壁走りのタイマー・終了処理 ---
        if (isWallRunning)
        {
            // ★追加: 移動キーから手を離したら強制的に壁走りを終了（落下）する
            if (!hasInput)
            {
                StopWallRun();
            }
            else
            {
                wallRunTimer -= Time.deltaTime;
                // 指定時間経過したら壁走りを終了し、落下を開始する
                if (wallRunTimer <= 0f)
                {
                    StopWallRun();
                }
            }
        }

        // --- ジャンプ・壁蹴り処理 ---
        if (jumpInput)
        {
            if (isGrounded)
            {
                // 通常ジャンプ
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else if (isWallRunning)
            {
                // 壁蹴りジャンプ (上方向と壁の垂直方向を足した斜め方向に飛ぶ)
                Vector3 wallJumpDirection = (Vector3.up + wallNormal).normalized;

                // ジャンプ前に現在のY軸の速度をリセットして勢いを安定させる
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(wallJumpDirection * wallJumpForce, ForceMode.Impulse);

                // ジャンプしたら壁走りを終了
                StopWallRun();
            }
        }

        // --- 移動と回転の処理 ---
        Vector3 direction = Vector3.zero;

        if (hasInput)
        {
            Vector3 cameraForward = Vector3.forward;
            Vector3 cameraRight = Vector3.right;

            if (mainCameraTransform != null)
            {
                cameraForward = mainCameraTransform.forward;
                cameraForward.y = 0f;
                cameraForward.Normalize();

                cameraRight = mainCameraTransform.right;
                cameraRight.y = 0f;
                cameraRight.Normalize();
            }

            direction = (cameraForward * vertical + cameraRight * horizontal).normalized;
        }

        // 壁走り中は進行方向を「壁に沿った方向」に制限する
        if (isWallRunning && direction.magnitude > 0.1f)
        {
            // Vector3.ProjectOnPlaneを使うことで、壁に向かって進む力を壁に沿って滑る力に変換します
            direction = Vector3.ProjectOnPlane(direction, wallNormal).normalized;
        }

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    // --- 衝突判定（壁の検知） ---

    private void OnCollisionStay(Collision collision)
    {
        // ★変更: 空中にいて、"Wall" に触れていて、かつ【移動入力がある場合】のみ壁走り判定を行う
        if (!isGrounded && collision.gameObject.CompareTag("Wall") && hasInput)
        {
            // まだ壁走りをしておらず、タイマーが残っているなら壁走りを開始
            if (!isWallRunning && wallRunTimer > 0f)
            {
                StartWallRun(collision);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 壁から離れたら壁走りを終了
        if (collision.gameObject.CompareTag("Wall"))
        {
            StopWallRun();
        }
    }

    // --- 壁走りの開始と終了の処理 ---

    private void StartWallRun(Collision collision)
    {
        isWallRunning = true;
        rb.useGravity = false; // 重力を無効化して落下を防ぐ

        // 接触した瞬間の落下の勢い（Y軸のマイナス速度）をゼロにする
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // 壁の法線（壁の向いている方向）を取得して保存
        wallNormal = collision.contacts[0].normal;
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true; // 重力を元に戻して再び落下させる
    }
}