using UnityEngine;

public class Circuit : MonoBehaviour
{
    [SerializeField]
    private Gate[] gates = new Gate[0];

    [SerializeField]
    private bool looping = true;

    private void Start()
    {
        // Enable the first gate.
        gates[0].ActivateGate(true);
        gates[0].SetGateLabel("START");

        // Disable all the other.
        for (int index = 1; index < gates.Length; index++)
        {
            gates[index].ActivateGate(false);
            gates[index].SetGateLabel(index.ToString());
        }

        // Register to gate passed event.
        Gate.OnPassed += OnGateTrigger;

        // Load our previous record (if we have any).
        Timer.Load();
    }

    private void OnDestroy()
    {
        // Unregister to gate passed event.
        Gate.OnPassed -= OnGateTrigger;

		// Stop Timer if running
		if (Timer.IsRunning)
		{
			Timer.Stop();
			Timer.Reset();
		}
	}

    private void OnGateTrigger(Gate gate)
    {
        int currentGateIndex = System.Array.IndexOf(gates, gate);
        int nextGateIndex = (currentGateIndex + 1) % gates.Length;
        int lastGateIndex = gates.Length - 1;

        if (Timer.IsRunning)
        {
            // Finish ?
            // Either it's:
            // - The first gate and we're looping.
            // - The last gate and we're not looping.
            if ((currentGateIndex == 0 && looping) || (currentGateIndex == lastGateIndex && !looping))
            {
                // Finish //

                Timer.Step();
                Timer.Stop();
                Timer.Save();
            }
            else
            {
                // Step //

                Timer.Step();

                // Disable current gate.
                gate.ActivateGate(false);

                // Enable next gate.
                Gate nextGate = gates[nextGateIndex];
                nextGate.ActivateGate(true);
            }
        }
        else
        {
            // Start //

            Timer.Reset();
            Timer.Start();

            // Disable current gate.
            gate.ActivateGate(false);

            if (looping)
            {
                gate.SetGateLabel("FINISH");
            }

            // Enable next gate.
            Gate nextGate = gates[nextGateIndex];
            nextGate.ActivateGate(true);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (gates != null)
        {
            Gizmos.color = Color.cyan;
            int gateCount = looping ? gates.Length : gates.Length - 1;
            for (int index = 0; index < gateCount; index++)
            {
                int nextIndex = (index + 1) % gates.Length;
                if (gates[index] != null && gates[nextIndex] != null)
                {
                    Gizmos.DrawLine(gates[index].transform.position, gates[nextIndex].transform.position);
                }
            }
        }
    }
#endif
}
