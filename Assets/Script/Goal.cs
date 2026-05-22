using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 触れたのがプレイヤーなら
        if (other.CompareTag("Player"))
        {
            // GameManagerに「ゲームクリア」を報告する！
            GameManager.instance.GameClear();
        }
    }
}