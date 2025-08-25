using UnityEngine;

public class FloorCheckTest : MonoBehaviour
{
	void Update()
	{
		Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Floor"));

		if (hit != null)
		{
			Debug.Log("è∞ÇÃè„: " + hit.name + " / Tag: " + hit.tag);
		}
		else
		{
			Debug.Log("è∞Ç∂Ç·Ç»Ç¢");
		}
	}
}
