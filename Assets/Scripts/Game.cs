using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using System.Collections;

public class Game : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private GameObject tutorialPanel = null;

	[Header("Day/Night System")]
	[SerializeField] private Light directionalLight;

	[System.Serializable]
	public class LightPreset
	{
		public Color color = Color.white;
		public float intensity = 1f;
		public Vector3 rotationEuler;
	}

	[SerializeField] private LightPreset[] presets;

	[Header("Timing")]
	[SerializeField] private float minTimeBetweenChanges = 10f;
	[SerializeField] private float maxTimeBetweenChanges = 30f;
	[SerializeField] private float transitionDuration = 5f;

	private readonly float restartInputHoldTime = 0.25f;
	private float currentRestartInputHoldTime = 0.0f;

	private void Start()
	{
		Gate.OnPassed += Gate_OnPassed;

		// Start random lighting loop
		if (directionalLight != null && presets.Length > 0)
		{
			StartCoroutine(RandomLightRoutine());
		}
	}

	private void Gate_OnPassed(Gate gate)
	{
		tutorialPanel.SetActive(false);
		Gate.OnPassed -= Gate_OnPassed;
	}

	private void Update()
	{
		// Restart
		KeyControl restartKey = Keyboard.current.rKey;

		if (restartKey.isPressed)
		{
			currentRestartInputHoldTime += Time.deltaTime;
		}
		else
		{
			currentRestartInputHoldTime = 0.0f;
		}

		if (currentRestartInputHoldTime >= restartInputHoldTime)
		{
			Scene currentScene = gameObject.scene;
			SceneManager.LoadScene(currentScene.name);
		}
	}

	// -------------------------
	// DAY / NIGHT SYSTEM
	// -------------------------

	private IEnumerator RandomLightRoutine()
	{
		while (true)
		{
			float waitTime = Random.Range(minTimeBetweenChanges, maxTimeBetweenChanges);
			yield return new WaitForSeconds(waitTime);

			LightPreset target = presets[Random.Range(0, presets.Length)];
			yield return StartCoroutine(TransitionToPreset(target));
		}
	}

	private IEnumerator TransitionToPreset(LightPreset target)
	{
		Color startColor = directionalLight.color;
		float startIntensity = directionalLight.intensity;
		Quaternion startRotation = directionalLight.transform.rotation;

		Quaternion targetRotation = Quaternion.Euler(target.rotationEuler);

		float time = 0f;

		while (time < transitionDuration)
		{
			float t = time / transitionDuration;

			directionalLight.color = Color.Lerp(startColor, target.color, t);
			directionalLight.intensity = Mathf.Lerp(startIntensity, target.intensity, t);
			directionalLight.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

			time += Time.deltaTime;
			yield return null;
		}

		// Ensure exact final values
		directionalLight.color = target.color;
		directionalLight.intensity = target.intensity;
		directionalLight.transform.rotation = targetRotation;
	}
}