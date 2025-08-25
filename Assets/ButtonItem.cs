using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonItem : MonoBehaviour
{
	[SerializeField] private string nextSceneName; // 遷移先のシーン名

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("ボタンに触れた！シーン遷移します。");
			SceneManager.LoadScene(nextSceneName);
		}
	}
}
