using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]

// This script defines a trigger volume that switches the active Cinemachine camera when the player enters it.
// It uses a world-oriented OverlapBox (with editable local rotation) to detect when the player enters the volume,
// and then it calls the CameraSwitcher to switch to the specified camera.
public class CameraTriggerVolume : MonoBehaviour
{
	[SerializeField] private CinemachineCamera cam;
	[SerializeField] private Vector3 boxSize;
	[SerializeField] private Vector3 boxCenter = Vector3.zero; // local center of the box
	[SerializeField] private Vector3 boxRotationEuler = Vector3.zero; // local rotation (degrees) applied to the box

	BoxCollider box;
	Rigidbody rb;
	bool playerInside;

	private void Awake()
	{
		box = GetComponent<BoxCollider>();
		rb = GetComponent<Rigidbody>();

		// Keep the scene's BoxCollider for authoring, but disable its physics trigger
		// and drive detection with a world-space oriented OverlapBox so we can apply an independent rotation.
		box.isTrigger = false;
		box.enabled = false;

		box.size = boxSize;
		box.center = boxCenter;

		rb.isKinematic = true;
	}

	private void FixedUpdate()
	{
		// Build oriented box in world-space using the local rotation field.
		Quaternion orientation = transform.rotation * Quaternion.Euler(boxRotationEuler);
		Vector3 worldCenter = transform.TransformPoint(box.center);

		// half extents must include lossy scale
		Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, transform.lossyScale);

		Collider[] hits = Physics.OverlapBox(worldCenter, halfExtents, orientation);
		bool found = false;
		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].CompareTag("Player"))
			{
				found = true;
				break;
			}
		}

		if (found && !playerInside)
		{
			playerInside = true;
			if (CameraSwitcher.ActiveCamera != cam) CameraSwitcher.SwitchCamera(cam);
		}
		else if (!found && playerInside)
		{
			playerInside = false;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		// Draw oriented box using transform + local rotation and lossy scale so the gizmo matches the OverlapBox used at runtime.
		Matrix4x4 oldMatrix = Gizmos.matrix;
		Matrix4x4 gizmoMatrix = Matrix4x4.TRS(transform.TransformPoint(boxCenter), transform.rotation * Quaternion.Euler(boxRotationEuler), transform.lossyScale);
		Gizmos.matrix = gizmoMatrix;
		Gizmos.DrawWireCube(Vector3.zero, boxSize);
		Gizmos.matrix = oldMatrix;
	}
}
