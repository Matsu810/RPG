using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wizardCon : MonoBehaviour
{
	[SerializeField] private int maxHP = 5;   // 最大HP
	[SerializeField] private float dashSpeed = 15f;
	[SerializeField] private float dashDuration = 0.20f; // ダッシュ時間
	[SerializeField] private float doubleTapTime = 0.3f; // 2回押しの間隔
	[SerializeField] private float JumpSpeed = 5.0f;
	[SerializeField] private float MoveSpeed = 5.5f;
	[SerializeField] private float gravityScale = 2f;
	[SerializeField] private float invincibleTime = 1.5f; // 無敵時間
	[SerializeField] private float flashInterval = 0.1f;  // 点滅間隔
	[SerializeField] private TextMeshProUGUI hpText;
	[SerializeField] private int jumpcount = 0;

	private int currentHP;                    // 現在HP
	private bool isDead = false;
	private Rigidbody2D rb;
	private SpriteRenderer playerSprite;
	private bool isGrounded = true;
    private Animator animator; // プレイヤーのアニメーター
    private float lastLeftTapTime = -1f;
	private float lastRightTapTime = -1f;
	private bool dashUsed = false;
	private bool isDashing = false;
	private float dashTimer = 0f;
	private float originalGravity;
	private bool isInvincible = false;

	[SerializeField] private GameObject magicPrefab;
	[SerializeField] private Transform magicPoint;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		playerSprite = GetComponent<SpriteRenderer>();
		rb.gravityScale = gravityScale;
		originalGravity = gravityScale;
		currentHP = maxHP; // HP初期化
		UpdateHPUI(); // UI更新
        animator = GetComponent<Animator>(); // アニメーターを取得
    }

    // Updateメソッド内のyield returnの使用を修正
    void Update()
    {
        float moveInput = isDashing ? 0f : Input.GetAxis("Horizontal");

        if (!isDashing)
        {
            // 入力があるときだけ速度を更新（慣性を殺さない）
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                //Runアニメーション開始
                animator.SetBool("isRun", true);
                //Jumpcountが1のときのみジャンプアニメーションを停止
                if (jumpcount == 1)
                {
                    animator.SetBool("isRun", false);
                    //Jumpアニメーション再生中でなければRunアニメーションを再生(if)
                    if(!animator.GetBool("isJump"))
                    {
                        animator.SetBool("isRun", true);
                    }
                    
                }
                rb.velocity = new Vector2(moveInput * MoveSpeed, rb.velocity.y);
            
            }
            else
            {
                //Runアニメーション停止
                animator.SetBool("isRun", false);
                // 入力がないときは慣性で自然減速
                float decelRate = 5f; // 減速スピード（大きいほど早く止まる）
                float newX = Mathf.MoveTowards(rb.velocity.x, 0f, decelRate * Time.deltaTime);
                rb.velocity = new Vector2(newX, rb.velocity.y);
            }

            // 左右の向き反転
            if (moveInput != 0)
            {
                // プレイヤー本体の反転
                Vector3 scale = transform.localScale;
                scale.x = moveInput > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                transform.localScale = scale;

                // MagicPointの位置も反転
                Vector3 magicPos = magicPoint.localPosition;
                magicPos.x = Mathf.Abs(magicPos.x) * (scale.x > 0 ? 1 : -1);
                magicPoint.localPosition = magicPos;
            }
        }
        else
        {
            


            // ダッシュ中は速度を維持
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                EndDash(); // ← EndDash では velocity を上書きしない
            }
        }

        // 空中でのダブルタップ判定
        if (!dashUsed && !isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                if (Time.time - lastLeftTapTime <= doubleTapTime)
                {
                    StartDash(-1);
                    dashUsed = true;
                }
                lastLeftTapTime = Time.time;
            }
            //Sキーを押した場合急降下する
            if (Input.GetKeyDown(KeyCode.S))
            {
                rb.velocity = new Vector2(rb.velocity.x, -dashSpeed * 2f); // 急降下速度
               
            }
            //Sキーを押した場合急降下する

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                if (Time.time - lastRightTapTime <= doubleTapTime)
                {
                    StartDash(1);
                    dashUsed = true;
                }
                lastRightTapTime = Time.time;
            }
        }

        // ジャンプ
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //既にjumpcountが1ならジャンプアニメーションを開始しない
            if (jumpcount == 0)
            {
                // ジャンプアニメーション開始
                animator.SetBool("isJump", true);
                //1ループ目のジャンプアニメーションが終わるとアニメーションを停止する

                Debug.Log("Jump animation started");
            }
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
            isGrounded = false;
            jumpcount++;
            // 0.25秒後にジャンプアニメーションを終了するコルーチンを開始
            StartCoroutine(EndJumpAnimation());
        }

        // 魔法発射
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot(magicPrefab);
        }

        // 落下速度の調整（通常時のみ）
        if (!isDashing)
        {
            if (rb.velocity.y < 0)
                rb.gravityScale = gravityScale * 0.5f;
            else
                rb.gravityScale = gravityScale;
        }
    }

    // ジャンプアニメーション終了用コルーチン
    private IEnumerator EndJumpAnimation()
    {
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("isJump", false);
    }



	private void StartDash(int direction)
	{
		isDashing = true;
		dashTimer = dashDuration;

		rb.gravityScale = 0f; // 重力オフ
		float upwardBoost = 1.5f;
		rb.velocity = new Vector2(direction * dashSpeed, upwardBoost);
	}

	private void EndDash()
	{
		isDashing = false;
		rb.gravityScale = originalGravity; // 重力戻す
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
	}
  
    private void OnCollisionEnter2D(Collision2D collision)
	{
		// 何かに当たったらダッシュ終了
		if (isDashing)
		{
			EndDash();
		}

		// Ground に当たったらリセット
		if (collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
			jumpcount = 0;
         //   yield return new WaitForSeconds(0.25f); // 0.5秒待つ
            animator.SetBool("isJump", false); // ジャンプアニメーション終了
            dashUsed = false;
		}
		
	}
	private void TakeDamage(int damage)
	{
		if (isDead) return; // すでに死亡済みなら無視

		currentHP -= damage;
        
        Debug.Log("Player HP: " + currentHP);
		UpdateHPUI();
		// 上方向に吹っ飛ぶ（ジャンプと同じ強さ）
		rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
        

        if (currentHP <= 0)
		{
			Die();
		}
		// 無敵状態にして点滅開始
		StartCoroutine(DamageFlash());
	}
    private void UpdateHPUI()
    {
        if (hpText != null)
        {
            // HPが0以下ならGame Over表示
            if (currentHP <= 0)
            {
                hpText.text = "HP: 0 Restart[R] or Exit[E] key";
                Debug.Log("Game Over");
            }
            else
            {
                hpText.text = "HP: " + currentHP.ToString();
                Debug.Log("Game");
            }
        }
    }
	private IEnumerator DamageFlash()
	{
		isInvincible = true;
		float timer = 0f;
        // ダメージアニメーションを再生(Trigger)
        animator.SetBool("hurt", true);
        Debug.Log("Player hit by enemy!");
        yield return new WaitForSeconds(0.25f); // 0.5秒待つ
        // ダメージアニメーションが終わったらフラグを戻す
        animator.SetBool("hurt", false);
        while (timer < invincibleTime)
		{
			playerSprite.enabled = !playerSprite.enabled; // 点滅
			yield return new WaitForSeconds(flashInterval);
			timer += flashInterval;
		}

		playerSprite.enabled = true; // 最後に表示を戻す
		isInvincible = false;
	}
	private void Die()
	{
		isDead = true;
        // 死亡アニメーションを再生
        animator.SetBool("die", true);
        // このスクリプトを止める
        this.enabled = false;
		// レンダリング消去
		playerSprite.enabled = false;

		// TODO: ゲームオーバーUIを表示したりリトライ処理を入れる余地あり
	}
	private void Shoot(GameObject magicPrefab)
	{
		GameObject magic = Instantiate(magicPrefab, magicPoint.position, Quaternion.identity);
		Rigidbody2D magicRb = magic.GetComponent<Rigidbody2D>();

		if (magicRb != null)
		{
			float direction = transform.localScale.x > 0 ? 1 : -1;
			magicRb.velocity = new Vector2(direction * 10f, 0f);
		}
	}
    // 修正: Unityメッセージの正しい大文字小文字（OnTriggerEnter2D）に変更し、未使用パラメータ警告を抑制
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //敵やボスに触れたら被弾
        
        if (!isInvincible && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss")))
        {

           
            TakeDamage(1);
            
        }
    }
}
