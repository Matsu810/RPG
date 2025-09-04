using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //BossDiedをGameManagerに移動
    public static GameManager Instance { get; private set; }
    private bool gotX = false;
    private bool gotY = false;
    private bool gotZ = false;

    // BossDiedフラグを追加
    private bool bossDied = false;
    public bool BossDied => bossDied;

    public bool DoorUnlocked => gotX && gotY && gotZ;

  
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject);
        }

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            //タイトルシーンに戻る
            SceneManager.LoadScene("TitleScene");
        }
        if (GameManager.Instance.BossDied)
        {
            // ドアを開ける処理
            Debug.Log("倒したボスが追いかけてくるぞ！");


        }
        else
        {
            Debug.Log("ドアは閉じたままです。");
            // ドアを閉じたまま
        }

    }
    //ボスが倒されているか

    public void CollectItem(string itemName)
    {
        switch (itemName)
        {
            case "X":
                gotX = true;
                break;
            case "Y":
                gotY = true;
                break;
            case "Z":
                gotZ = true;
                break;
        }

        Debug.Log($"Item {itemName} collected!");
    }

    // Bossが倒されたときに呼び出す
    public void SetBossDied()
    {
        bossDied = true;
        Debug.Log("Bossが倒されました！");
    }

    // ドアを開けたあとに呼び出す
    public void ResetKeys()
    {
        gotX = false;
        gotY = false;
        gotZ = false;
        Debug.Log("鍵がリセットされました。新しい部屋で再挑戦！");
    }
}
