using UnityEngine;

public class Item : MonoBehaviour
{
	public string itemName; // "X" "Y" "Z"

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			GameManager.Instance.CollectItem(itemName);
			Destroy(gameObject); // éÊÇ¡ÇΩÇÁè¡Ç¶ÇÈ
		}
	}
}
