using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        // Playerタグのオブジェクトだけ反応
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}