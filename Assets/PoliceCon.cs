using UnityEngine;
using System.Collections;

public class PoliceCon : MonoBehaviour
{
	public float moveSpeed = 5f;
	public float jumpForce = 7f;

	public GameObject attackCollider;    

	public float attackDuration = 0.3f;

	private Rigidbody rb;
	private bool isGrounded = false;
	private bool isAttacking = false;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		if (attackCollider != null)
		{
			attackCollider.SetActive(false);
		}
	}

	void Update()
	{
		Move();
		Jump();

		if (Input.GetKeyDown(KeyCode.F) && !isAttacking)
		{
			StartCoroutine(DoAttack());
		}
	}

	void Move()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		Vector3 dir = new Vector3(h, 0f, v).normalized;

		if (dir.magnitude > 0)
		{
			transform.position += dir * moveSpeed * Time.deltaTime;
			// transform.forward = dir;
		}
	}

	void Jump()
	{
		if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			isGrounded = false;
		}
	}

	IEnumerator DoAttack()
	{
		isAttacking = true;

		if (attackCollider != null)
		{
			attackCollider.SetActive(true);
		}

		yield return new WaitForSeconds(attackDuration);

		if (attackCollider != null)
		{
			attackCollider.SetActive(false);
		}

		isAttacking = false;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
		}
	}

	//  GizmoÇï`âÊÇ∑ÇÈÅiInspectorÇ≈ëIëíÜÇÃÇ›Åj
	void OnDrawGizmosSelected()
	{
		if (attackCollider != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(attackCollider.transform.position, attackCollider.transform.localScale);
		}
	}
}
