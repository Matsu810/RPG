using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 2.0f; // 敵の移動速度
    [SerializeField] private MoveType moveType = MoveType.Vertical; // 移動タイプの選択
    [SerializeField] private float moveRange = 5.0f; // 移動範囲（Tracking以外で使用）
    private Rigidbody2D rb;
    private SpriteRenderer enemySprite;
    public float gravityScale = 2f; // 重力スケール
    private bool isGrounded = true;// 地面を踏んでいるかどうかのフラグ
    public enum MoveType
    {
        Vertical,
        Horizontal,
        Tracking,
    }
   
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dコンポーネントを取得
        enemySprite = GetComponent<SpriteRenderer>();
        rb.gravityScale = gravityScale; // Rigidbody2Dの重力スケールを設定
    }

    // Update is called once per frame
    void Update()
    {
        //switch文とvelocityを使った移動処理
        switch (moveType)
        {
            case MoveType.Vertical:
                //moveRangeの分だけ移動
                rb.velocity = new Vector2(rb.velocity.x, MoveSpeed);
                break;
            case MoveType.Horizontal:
                rb.velocity = new Vector2(MoveSpeed, rb.velocity.y);
                break;
            case MoveType.Tracking:
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Vector2 direction = (player.transform.position - transform.position).normalized;
                    rb.velocity = direction * MoveSpeed;
                }
                break;
        }
        

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            Debug.Log("Enemy hit by attack!"); // 攻撃に当たったらログを出力
            Destroy(gameObject); // 攻撃に当たったら敵を削除
        }
    }
}
