using UnityEngine;

[DisallowMultipleComponent]
public class Headlights : MonoBehaviour
{
	[Header("Input")]
	public KeyCode toggleKey = KeyCode.E;

	[Header("Lights (Assign Left & Right)")]
	public Light leftHeadlight;
	public Light rightHeadlight;

	bool _isOn;

	public bool IsOn => _isOn;

	void Reset()
	{
		// Try auto-assigning first two lights found in children
		var lights = GetComponentsInChildren<Light>();
		if (lights.Length >= 2)
		{
			leftHeadlight = lights[0];
			rightHeadlight = lights[1];
		}
	}

	void Start()
	{
		// Safety fallback
		if (leftHeadlight == null || rightHeadlight == null)
		{
			var lights = GetComponentsInChildren<Light>();
			if (lights.Length >= 2)
			{
				leftHeadlight = lights[0];
				rightHeadlight = lights[1];
			}
		}

		ApplyLightState();
	}

	void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			Toggle();
			Debug.Log($"Headlights toggled via input: {toggleKey}");
		}
	}

	public void Toggle()
	{
		SetState(!_isOn);
		Debug.Log($"Headlights turned {(_isOn ? "ON" : "OFF")}");
	}

	public void SetState(bool on)
	{
		_isOn = on;
		ApplyLightState();
	}

	void ApplyLightState()
	{
		if (leftHeadlight != null)
			leftHeadlight.enabled = _isOn;

		if (rightHeadlight != null)
			rightHeadlight.enabled = _isOn;
	}
}