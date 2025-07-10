using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCon : MonoBehaviour
{
    [SerializeField] private bool Jumpflag;
    [SerializeField] private float JumpSpeed = 5.0f;
    [SerializeField] private float MoveSpeed = 5.5f;
    private int jumpcount = 0;
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    public float gravityScale = 0f; // 重力スケール
    private bool isGrounded = true;// 地面を踏んでいるかどうかのフラグ
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dコンポーネントを取得
        playerSprite = GetComponent<SpriteRenderer>();
        rb.gravityScale = gravityScale; // Rigidbody2Dの重力スケールを設定
    }

    // Update is called once per frame
    void Update()
    {
        
       
       
    }

    private void FixedUpdate()
    {
        //上下の入力を取得
        float verticalInput = Input.GetAxis("Vertical"); // 上下の入力を取得

        float moveInput = Input.GetAxis("Horizontal"); // 左右の入力を取得
        rb.velocity = new Vector2(moveInput, verticalInput) * MoveSpeed * Time.deltaTime; // 上下方向の速度を設定

        // 左右の入力に応じてキャラクターの向きを変更
        if (moveInput != 0) playerSprite.flipX = moveInput < 0; // 左向きならスプライトを反転
        int flipPoint = playerSprite.flipX ? -1 : 1; // プレイヤーの向きに応じてフリップポイントを設定
    }
}
