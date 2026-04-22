using UnityEngine;

public class CheckPointUpdate : MonoBehaviour
{
    public Transform relocationPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            CheckpointMarker[] checkpoints = FindObjectsByType<CheckpointMarker>();

            if (checkpoints.Length > 1)
            {
                Debug.LogError("Multiple respawn locations found. Checkpoint update failed.");
            }
            else if (checkpoints.Length == 0)
            {
                Debug.LogError("No respawn location found in the scene. Checkpoint update failed.");
            }
            else
            {
                checkpoints[0].transform.position = relocationPoint.position;
                checkpoints[0].transform.rotation = relocationPoint.rotation;
            }
        }
    }
}
