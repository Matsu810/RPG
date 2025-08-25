using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;   
	public float smoothSpeed = 0.125f; // カメラの追従スピード
	public Vector3 offset;      // プレイヤーからのオフセット（カメラの高さなど）

	void LateUpdate()
	{
		if (target == null) return;

		// プレイヤーの位置にオフセットを足した目標地点
		Vector3 desiredPosition = target.position + offset;

		// スムーズにカメラを移動
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

		// Z軸はカメラのものを固定
		smoothedPosition.z = transform.position.z;

		transform.position = smoothedPosition;
	}
}
