using UnityEngine;

public class StartFinishTrigger : MonoBehaviour
{
    public LapsManager lapsManager;
    private bool hasStarted = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hasStarted)
            {
                lapsManager.StartTimer();
                hasStarted = true;
                Debug.Log("Timer Started!");
            }
            else
            {
                // Only complete the lap if checkpoint was passed
                if (lapsManager.hasPassedCheckpoint)
                {
                    lapsManager.CompleteLap();
                    Debug.Log("Lap completed!");
                }
                else
                {
                    Debug.Log("Cannot complete lap: Checkpoint not passed yet.");
                }
            }
        }
    }
}
