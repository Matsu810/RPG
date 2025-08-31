using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5.0f;   // 移動速度
	[SerializeField] private Transform groundCheck;    // 足元チェック
	[SerializeField] private Transform wallCheck;      // 前方チェック
	[SerializeField] private float checkDistance = 0.5f; // 判定距離
	[SerializeField] private float flipCooldown = 0.2f; // クールタイム

	private Rigidbody2D rb;
	private SpriteRenderer enemySprite;
	private float direction = 1f; // 移動方向
	private float flipTimer = 0f; // クールタイム管理

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		enemySprite = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		Patrol();
	}

	private void Patrol()
	{
		rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

		if (flipTimer > 0)
		{
			flipTimer -= Time.deltaTime;
			return; // クールタイム中は判定しない
		}

		// 足元のチェック（崖判定）
		RaycastHit2D groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance);
		// 前方のチェック（壁判定）
		RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, Vector2.right * direction, checkDistance);

		if (groundHit.collider == null || (wallHit.collider != null && wallHit.collider.CompareTag("Ground")))
		{
			Flip();
		}
	}

	private void Flip()
	{
		direction *= -1;
		enemySprite.flipX = direction < 0;
		flipTimer = flipCooldown; // 反転後はしばらく判定無効
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Attack"))
		{
			Debug.Log("Enemy hit by attack!");
			Destroy(gameObject);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (groundCheck != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * checkDistance);
		}
		if (wallCheck != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * direction * checkDistance);
		}
	}
}
