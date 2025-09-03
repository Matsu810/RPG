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

	private int currentHP;                    // 現在HP
	private bool isDead = false;
	private int jumpcount = 0;
	private Rigidbody2D rb;
	private SpriteRenderer playerSprite;
	private bool isGrounded = true;

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
	}

	void Update()
	{
		float moveInput = Input.GetAxis("Horizontal");

		if (!isDashing)
		{
			// 入力があるときだけ速度を更新（慣性を殺さない）
			if (Mathf.Abs(moveInput) > 0.01f)
			{
				rb.velocity = new Vector2(moveInput * MoveSpeed, rb.velocity.y);
			}
			else
			{
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
			rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
			isGrounded = false;
			jumpcount++;
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
			dashUsed = false;
		}
		// 敵やボスに触れたら被弾
		if (!isInvincible && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss")))
		{
			TakeDamage(1);
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
			hpText.text = "HP: " + currentHP.ToString();
		}
	}
	private IEnumerator DamageFlash()
	{
		isInvincible = true;
		float timer = 0f;

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
		// このスクリプトを止める
		this.enabled = false;
		// レンダリング消去
		playerSprite.enabled = false;
		if (hpText != null)
			hpText.text = "HP: 0";
		// デバッグログ
		Debug.Log("Game Over");

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

}
