using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wizardCon : MonoBehaviour
{
    [SerializeField] private bool Jumpflag;
    [SerializeField] private float JumpSpeed = 5.0f;
    [SerializeField] private float MoveSpeed = 5.5f;
    private int jumpcount = 0;
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    public float gravityScale = 2f; // 重力スケール
    private bool isGrounded = true;// 地面を踏んでいるかどうかのフラグ

    //魔法の弾のプレハブ
    [SerializeField] private GameObject magicPrefab;
    //魔法の弾の発射位置
    [SerializeField] private Transform magicPoint;
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
        //Velocityを用いた移動処理
        float moveInput = Input.GetAxis("Horizontal"); // 左右の入力を取得
        rb.velocity = new Vector2(moveInput * MoveSpeed, rb.velocity.y); // 水平方向の速度を設定
        // 左右の入力に応じてキャラクターの向きを変更
        if(moveInput != 0) playerSprite.flipX = moveInput < 0; // 左向きならスプライトを反転
        int flipPoint = playerSprite.flipX ? -1 : 1; // プレイヤーの向きに応じてフリップポイントを設定
        magicPoint.localPosition = new Vector2(flipPoint * Mathf.Abs(magicPoint.localPosition.x), magicPoint.localPosition.y);

        // ジャンプ処理
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
            isGrounded = false; // ジャンプしたので地面を踏んでいない
            jumpcount++;
        }

        // 魔法の弾を発射する処理
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot(magicPrefab);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // 地面に接触したので地面を踏んでいる
            jumpcount = 0; // ジャンプカウントをリセット
        }
    }
    private void Shoot(GameObject magicPrefab)
    {
        // 魔法の弾を生成
        GameObject magic = Instantiate(magicPrefab, magicPoint.position, Quaternion.identity);
        Rigidbody2D magicRb = magic.GetComponent<Rigidbody2D>();
        SpriteRenderer sprite = magic.GetComponent<SpriteRenderer>(); // スプライトを取得（必要に応じて）
        sprite.flipX = playerSprite.flipX; // プレイヤーの向きに応じてスプライトを反転
        if (magicRb != null)
        {
            // 魔法の弾に力を加える

            float direction = playerSprite.flipX ? -1: 1; // プレイヤーの向きに応じて方向を決定
            //弾を射出
            magicRb.velocity = new Vector2(direction * 10f, 0f); // プレイヤーの向きに応じて発射

            //magicRb.velocity = new Vector2(10f, 0f); // 右方向に発射
        }
    }
}
