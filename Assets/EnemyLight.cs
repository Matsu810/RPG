using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyLight : MonoBehaviour
{
	public float rotationSpeed = 45f; // ライトが回転する速度
	private float playerStayTime = 0f;
	private bool playerInside = false;

	void Update()
	{
		// ライト（このオブジェクト）を回転させる
		transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

		// プレイヤーがライト内にいるなら時間をカウント
		if (playerInside)
		{
			playerStayTime += Time.deltaTime;

			if (playerStayTime >= 0.5f)
			{
				Debug.Log("アウト！");
				playerStayTime = 0f; // 一度だけ表示するならこの行は不要
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerInside = true;
			playerStayTime = 0f; // 入った瞬間にリセット
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerInside = false;
			playerStayTime = 0f;
		}
	}
}
