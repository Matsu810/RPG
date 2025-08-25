using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wizardCon : MonoBehaviour
{
	[SerializeField] private float dashSpeed = 15f;
	[SerializeField] private float dashDuration = 0.20f; // ダッシュ時間
	[SerializeField] private float doubleTapTime = 0.3f; // 2回押しの間隔
	[SerializeField] private float JumpSpeed = 5.0f;
	[SerializeField] private float MoveSpeed = 5.5f;
	[SerializeField] private float gravityScale = 2f;

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

	[SerializeField] private GameObject magicPrefab;
	[SerializeField] private Transform magicPoint;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		playerSprite = GetComponent<SpriteRenderer>();
		rb.gravityScale = gravityScale;
		originalGravity = gravityScale;
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
			if (moveInput != 0) playerSprite.flipX = moveInput < 0;
			int flipPoint = playerSprite.flipX ? -1 : 1;
			magicPoint.localPosition = new Vector2(flipPoint * Mathf.Abs(magicPoint.localPosition.x), magicPoint.localPosition.y);
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
	}

	private void Shoot(GameObject magicPrefab)
	{
		GameObject magic = Instantiate(magicPrefab, magicPoint.position, Quaternion.identity);
		Rigidbody2D magicRb = magic.GetComponent<Rigidbody2D>();
		SpriteRenderer sprite = magic.GetComponent<SpriteRenderer>();
		sprite.flipX = playerSprite.flipX;
		if (magicRb != null)
		{
			float direction = playerSprite.flipX ? -1 : 1;
			magicRb.velocity = new Vector2(direction * 10f, 0f);
		}
	}
}
