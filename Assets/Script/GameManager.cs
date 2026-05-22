using UnityEngine;
using UnityEngine.SceneManagement; // シーン移動に必要
using System.Collections; // 「〇秒待つ」処理（コルーチン）に必要

public class GameManager : MonoBehaviour
{
    // どこからでもGameManagerを呼べるようにする便利な仕組み
    public static GameManager instance;

    [Header("ゲームの設定")]
    public int score = 0; // スコア
    public string nextStageName = "Stage2"; // 次のステージ名

    [Header("UIの設定")]
    public GameObject clearUI; // クリア時に表示するUI（文字や画像など）

    private void Awake()
    {
        // ゲーム開始時に、自分自身をinstanceにセットする
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        // 最初はクリア画面を非表示にしておく
        if (clearUI != null)
        {
            clearUI.SetActive(false);
        }
    }

    // スコアを追加する機能（今回は準備だけ）
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("現在のスコア: " + score);
    }

    // ゴールした時にGoalスクリプトから呼ばれる処理
    public void GameClear()
    {
        Debug.Log("ゲームクリア！");

        // クリア画面を表示する
        if (clearUI != null)
        {
            clearUI.SetActive(true);
        }

        // 数秒待ってから次のステージへ行く処理をスタート
        StartCoroutine(GoToNextStage());
    }

    // 〇秒待ってからシーンを移動する処理（コルーチン）
    private IEnumerator GoToNextStage()
    {
        yield return new WaitForSeconds(3.0f); // 3秒待つ
        SceneManager.LoadScene(nextStageName); // 次のシーンへ
    }
}