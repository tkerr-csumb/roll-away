using UnityEngine;

public class CheckpointMarker : MonoBehaviour
{
    private Vector3 savedGravity = Vector3.down * 9.81f;

    public void SetSavedGravity(Vector3 newGravityDirection) {
        savedGravity = newGravityDirection;
    }

    public Vector3 GetSavedGravity() {
        return savedGravity;
    }
}
