using UnityEngine;

public class CheckpointMarker : MonoBehaviour
{
    private Vector3 gravityDirection = Vector3.down;

    public void SetGravityDirection(Vector3 newGravityDirection) {
        gravityDirection = newGravityDirection;
    }

    public Vector3 GetGravityDirection() {
        return gravityDirection;
    }
}
