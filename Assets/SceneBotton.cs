using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBotton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rキーでタイトルシーンに戻る
        if(Input.GetKeyDown(KeyCode.R))
        {
            //タイトルシーンに戻る
         SceneManager.LoadScene("TitleScene");
        }
        //スペースキーでゲームシーンに行く
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //ゲームシーンに行く
         SceneManager.LoadScene("TopGame");
        }
    }
}
