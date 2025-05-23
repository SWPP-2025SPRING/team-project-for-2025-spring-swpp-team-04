using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public LapsManager lapsManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && lapsManager != null)
        {
            lapsManager.hasPassedCheckpoint = true;
            lapsManager.ResetTimer();
        }
    }
}
