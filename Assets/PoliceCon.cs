using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoliceCon : MonoBehaviour
{
    [SerializeField] private bool Jumpflag;
    [SerializeField] private float JumpSpeed = 5.0f;
    [SerializeField] private float MoveSpeed = 5.5f;
    private int jumpcount = 0;
    private Rigidbody rb;
    private SpriteRenderer playerSprite;
    public float gravityScale = 0f; // 重力スケール
    private bool isGrounded = true;// 地面を踏んでいるかどうかのフラグ
    private bool isAttack = false; // 攻撃中かどうかのフラグ
    private float elapsedTime = 0f; // 経過時間

    private const float AttackDuration = 0.25f; // 攻撃の持続時間

    //攻撃する武器のプレハブ
    [SerializeField] private GameObject attackPrefab;
    //攻撃位置
    [SerializeField] private Transform AttackPoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody2Dコンポーネントを取得
        playerSprite = GetComponent<SpriteRenderer>();
       // rb.gravityScale = gravityScale; // Rigidbody2Dの重力スケールを設定
    }

    // Update is called once per frame
    void Update()
    {
        // ジャンプ処理
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
            isGrounded = false; // ジャンプしたので地面を踏んでいない
            jumpcount++;
        }
        // 攻撃する処理
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot(attackPrefab);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // 地面に接触したら地面を踏んでいる
            jumpcount = 0; // ジャンプカウントをリセット
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by enemy!"); // 敵に当たったらログを出力
        }
    }
    private void FixedUpdate()
    {
        if (isAttack)
        {
            elapsedTime += Time.deltaTime; // 経過時間を更新
            if (elapsedTime >= AttackDuration)
            {
                isAttack = false; // 攻撃が終了
                elapsedTime = 0f; // 経過時間をリセット
            }
            return;
        }

        //上下の入力を取得
        float verticalInput = Input.GetAxis("Vertical"); // 上下の入力を取得

        float moveInput = Input.GetAxis("Horizontal"); // 左右の入力を取得
        rb.velocity = new Vector2(moveInput, verticalInput) * MoveSpeed * Time.deltaTime; // 上下方向の速度を設定

        // 左右の入力に応じてキャラクターの向きを変更
        if (moveInput != 0) playerSprite.flipX = moveInput < 0; // 左向きならスプライトを反転
        int flipPoint = playerSprite.flipX ? -1 : 1; // プレイヤーの向きに応じてフリップポイントを設定
        AttackPoint.localPosition = new Vector2(flipPoint * Mathf.Abs(AttackPoint.localPosition.x), AttackPoint.localPosition.y);
    }
    private void Shoot(GameObject attackPrefab)
    {
        // 攻撃する武器を生成
        GameObject attack = Instantiate(attackPrefab, AttackPoint.position, Quaternion.identity);
        // 攻撃中は操作不可
        rb.velocity = Vector2.zero; // 攻撃中は移動を停止
        isAttack = true; // 攻撃中フラグを立てる

        SpriteRenderer sprite = attack.GetComponent<SpriteRenderer>(); // スプライトを取得（必要に応じて）
        sprite.flipX = playerSprite.flipX; // プレイヤーの向きに応じてスプライトを反転
        //0.5秒後に攻撃を削除
        Destroy(attack, AttackDuration);

    }
}
