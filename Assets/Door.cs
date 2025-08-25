using UnityEngine;

public class Door : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.CompareTag("Player"))
		{
			if (GameManager.Instance.DoorUnlocked)
			{
				Debug.Log("ドアが消えた！ プレイヤーは通過できる");
				Destroy(gameObject); // ドアを消す
				GameManager.Instance.ResetKeys();
			}
			else
			{
				Debug.Log("ドアはまだ開かない…");
			}
		}
	}
}
