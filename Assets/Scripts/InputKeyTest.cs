using UnityEngine;

public class InputKeyTest : MonoBehaviour
{
	public KeyCode testKey = KeyCode.E;

	void Start()
	{
		Debug.Log($"InputKeyTest Start - GameObject: {gameObject.name}, enabled: {enabled}");
	}

	void Update()
	{
		// Occasional heartbeat so console isn't spammed
		if (Time.frameCount % 300 == 0)
			Debug.Log("InputKeyTest Update heartbeat");

		if (Input.GetKeyDown(testKey))
			Debug.Log($"Detected key via Input.GetKeyDown: {testKey}");
	}
}