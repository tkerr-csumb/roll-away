using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private Transform respawnPoint;

    private void Start()
    {
        CheckpointMarker[] checkpoints = FindObjectsByType<CheckpointMarker>();

        if (checkpoints.Length == 0)
        {
            Debug.LogError("No respawn location found in the scene.");
        }
        else if (checkpoints.Length > 1)
        {
            Debug.LogError("Multiple respawn locations found.");
        }
        else
        {
            respawnPoint = checkpoints[0].transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;
            
            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            CheckpointMarker activeCheckpoint = respawnPoint.GetComponent<CheckpointMarker>();
            Vector3 savedGravity = activeCheckpoint.GetSavedGravity();

            if (other.TryGetComponent<GravityControl>(out GravityControl playerGravity))
            {
                playerGravity.SetGravity(savedGravity, true);
            }
            
        }
    }
}
