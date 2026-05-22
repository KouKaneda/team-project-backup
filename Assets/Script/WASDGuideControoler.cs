using UnityEngine;
// 新しいInput Systemを使うために必要
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public class WASDGuideController : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Header("フェードの設定")]
    public float fadeSpeed = 3f;
    public float delayBeforeShow = 2f;

    private float idleTimer = 0f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        // --- 新しいInput Systemでの入力検知 ---
        bool hasInput = false;

        // キーボードのいずれかのキーが押されているか確認
        if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
        {
            hasInput = true;
        }

        //// マウスの動きも入力に含める場合
        //if (Mouse.current != null && Mouse.current.delta.ReadValue().magnitude > 0.1f)
        //{
        //    hasInput = true;
        //}

        // --- フェード処理 ---
        if (hasInput)
        {
            // 入力があれば即座にタイマーをリセットし、フェードアウト
            idleTimer = 0f;
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
        }
        else
        {
            // 入力がない間はタイマーを進める
            idleTimer += Time.deltaTime;

            // 指定した秒数（delayBeforeShow）を過ぎたらフェードイン
            if (idleTimer >= delayBeforeShow)
            {
                canvasGroup.alpha += fadeSpeed * Time.deltaTime;
            }
        }

        // アルファ値を 0.0 〜 1.0 の間に固定
        canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha);
    }
}