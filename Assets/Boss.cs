using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 12f;
	[SerializeField] private float tackleDuration = 2f;
	[SerializeField] private float stunDuration = 2f;

	[SerializeField] private float jumpForce = 7f;
	[SerializeField] private GameObject bossShotPrefab;
	[SerializeField] private Transform shotPoint; // 発射位置
	[SerializeField] private float shotInterval = 0.75f;
	[SerializeField] private int maxShots = 3;
	[SerializeField] private float shotSpeed = 12f;

	private Rigidbody2D rb;
	private GameObject player;
	private SpriteRenderer spriteRenderer;
	private SpriteRenderer bossSprite;


	private enum ActionType { None, Tackle, Shoot }
	private ActionType lastAction = ActionType.None;
	private ActionType secondLastAction = ActionType.None;

	private bool isGrounded = true;
	private bool isAttacking = false;
	private bool isStunned = false;

	private int health = 10;

	private Coroutine flashRoutine;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		player = GameObject.FindGameObjectWithTag("Player");
		StartCoroutine(ActionLoop());
		bossSprite = GetComponent<SpriteRenderer>();
	}
	void Update()
	{
	
		UpdateFacing();
	}

	private IEnumerator ShootAtPlayer()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");

		if (player != null)
		{
			Vector2 direction = (player.transform.position - shotPoint.position).normalized;

			GameObject shot = Instantiate(bossShotPrefab, shotPoint.position, Quaternion.identity);
			Rigidbody2D shotRb = shot.GetComponent<Rigidbody2D>();
			if (shotRb != null)
			{
				float shotSpeed = 12f; // 弾速
				shotRb.velocity = direction * shotSpeed;
			}
		}

		yield return null;
	}

	private void UpdateFacing()
	{
		if (player == null) return;

		// プレイヤーが右にいれば右向き、左なら左向き
		if (player.transform.position.x > transform.position.x)
		{
			bossSprite.flipX = true; // 右向き
		}
		else
		{
			bossSprite.flipX = false;  // 左向き
		}
	}


	private IEnumerator ActionLoop()
	{
		while (true)
		{
			if (!isAttacking && !isStunned)
			{
				ActionType nextAction = DecideNextAction();

				if (nextAction == ActionType.Tackle)
				{
					yield return StartCoroutine(TackleAttack());
				}
				else if (nextAction == ActionType.Shoot)
				{
					yield return StartCoroutine(ShootAttack());
				}

				secondLastAction = lastAction;
				lastAction = nextAction;

				yield return new WaitForSeconds(1f);
			}
			yield return null;
		}
	}

	private ActionType DecideNextAction()
	{
		if (lastAction == secondLastAction && lastAction != ActionType.None)
		{
			return (lastAction == ActionType.Tackle) ? ActionType.Shoot : ActionType.Tackle;
		}

		return (Random.value < 0.5f) ? ActionType.Tackle : ActionType.Shoot;
	}

	private IEnumerator TackleAttack()
	{
		isAttacking = true;

		if (player != null)
		{
			Vector2 dir = (player.transform.position - transform.position).normalized;
			rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
		}

		yield return new WaitForSeconds(tackleDuration);

		rb.velocity = Vector2.zero;
		isAttacking = false;
	}

	private IEnumerator ShootAttack()
	{
		isAttacking = true;

		rb.velocity = new Vector2(rb.velocity.x, jumpForce);

		int shotsFired = 0;
		while (shotsFired < maxShots)
		{
			yield return new WaitForSeconds(shotInterval);

			if (player != null)
			{
				Vector2 dir = (player.transform.position - transform.position).normalized;
				GameObject shot = Instantiate(bossShotPrefab, shotPoint.position, Quaternion.identity);
				Rigidbody2D shotRb = shot.GetComponent<Rigidbody2D>();
				if (shotRb != null)
				{
					shotRb.velocity = dir * shotSpeed;
				}
			}

			shotsFired++;
		}

		yield return new WaitUntil(() => isGrounded);

		isAttacking = false;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// 着地判定
		if (collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
		}

		// 体当たり中に「Wall」に衝突したら硬直
		if (isAttacking && collision.gameObject.CompareTag("Wall"))
		{
			StartCoroutine(Stun());
		}

		// 攻撃を受けたらダメージ処理
		if (collision.gameObject.CompareTag("Attack"))
		{
			TakeDamage(1);
			Destroy(collision.gameObject);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = false;
		}
	}


	private IEnumerator Stun()
	{
		isAttacking = false;
		isStunned = true;

		// 体当たり直後は停止する代わりに、少し後ろにノックバック
		float knockbackDir = bossSprite.flipX ? 2 : -3; // 向いている方向と逆にノックバック
		rb.velocity = new Vector2(knockbackDir * 3f, rb.velocity.y);

		// 白点滅開始
		if (flashRoutine != null) StopCoroutine(flashRoutine);
		flashRoutine = StartCoroutine(Flash(Color.white, stunDuration));

		yield return new WaitForSeconds(stunDuration);

		isStunned = false;

		// 元の色に戻す
		spriteRenderer.color = Color.white;

		// 硬直解除後に完全に停止（再度動き出すのは次のActionLoopで決定）
		rb.velocity = Vector2.zero;
	}

	private void TakeDamage(int damage)
	{
		health -= damage;
		Debug.Log("Boss HP: " + health);

		if (isStunned)
		{
			// 硬直中に攻撃されたら赤点滅
			if (flashRoutine != null) StopCoroutine(flashRoutine);
			flashRoutine = StartCoroutine(Flash(Color.red, 0.5f));
		}

		if (health <= 0)
		{
			Destroy(gameObject);
		}
	}

	private IEnumerator Flash(Color flashColor, float duration)
	{
		float timer = 0f;
		bool visible = true;

		while (timer < duration)
		{
			spriteRenderer.color = visible ? flashColor : new Color(1f, 1f, 1f, 0f); // 点滅
			visible = !visible;

			yield return new WaitForSeconds(0.1f);
			timer += 0.1f;
		}

		spriteRenderer.color = Color.white; // 最後に戻す
	}
}
