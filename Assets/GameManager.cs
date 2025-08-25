using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	private bool gotX = false;
	private bool gotY = false;
	private bool gotZ = false;

	public bool DoorUnlocked => gotX && gotY && gotZ;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	public void CollectItem(string itemName)
	{
		switch (itemName)
		{
			case "X":
				gotX = true;
				break;
			case "Y":
				gotY = true;
				break;
			case "Z":
				gotZ = true;
				break;
		}

		Debug.Log($"Item {itemName} collected!");
	}

	// ドアを開けたあとに呼び出す
	public void ResetKeys()
	{
		gotX = false;
		gotY = false;
		gotZ = false;
		Debug.Log("鍵がリセットされました。新しい部屋で再挑戦！");
	}
}
