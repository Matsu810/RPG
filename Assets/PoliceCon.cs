using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PoliceCon : MonoBehaviour
{
	public float moveSpeed = 5f;
	public string horizontalAxis = "Horizontal";
	public string verticalAxis = "Vertical";

	Rigidbody2D rb;
	LayerMask floorMask;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0f;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;

		// Floorレイヤーだけを判定対象に
		floorMask = LayerMask.GetMask("Floor");
	}

	void FixedUpdate()
	{
		float h = Input.GetAxisRaw(horizontalAxis);
		float v = Input.GetAxisRaw(verticalAxis);

		Vector2 input = new Vector2(h, v);

		// 斜め移動の速度を一定に
		if (input.sqrMagnitude > 1f)
			input = input.normalized;

		// 移動先を計算
		Vector2 nextPos = rb.position + input * moveSpeed * Time.fixedDeltaTime;

		// 次の位置が床の上なら進む
		if (CanMoveTo(nextPos))
		{
			rb.velocity = input * moveSpeed;
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	bool CanMoveTo(Vector2 targetPos)
	{
		// 足元のちょっと下を判定
		float checkRadius = 0.05f;
		Collider2D hit = Physics2D.OverlapCircle(targetPos, checkRadius, floorMask);

		return hit != null && hit.CompareTag("floor");
	}
}
