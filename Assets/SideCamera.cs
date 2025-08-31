using UnityEngine;

public class SideCamera : MonoBehaviour
{
	[SerializeField] private Transform player; // プレイヤー
	[SerializeField] private float smoothSpeed = 0.125f; // 追従の滑らかさ
	[SerializeField] private Vector3 offset; // カメラ位置のオフセット（例: (0, 2, -10)）

	private void LateUpdate()
	{
		if (player == null) return;

		// プレイヤーのX,Y + オフセットをターゲットにする
		Vector3 targetPosition = new Vector3(
			player.position.x + offset.x,
			player.position.y + offset.y,
			transform.position.z // Zはカメラの固定値
		);

		// スムーズに追従
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

		transform.position = smoothedPosition;
	}
}
