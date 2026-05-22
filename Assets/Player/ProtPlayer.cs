using UnityEngine;
using UnityEngine.InputSystem;

public class ProtPlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1;

        // プレイヤー基準移動
        Vector3 move =
      transform.forward * input.y +
      transform.right * input.x;

        transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }
}

