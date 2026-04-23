using UnityEngine;

public class GravityArrowPrefab : MonoBehaviour {
    public enum GravityDirections{ Up, Down, Left, Right, Forward, Backward,}
    
    [SerializeField] private GravityDirections direction;
    private float gravForce = 9.81f;
    // private float itemTimerMax 
    // private float itemTimer

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GravityControl cg = other.GetComponent<GravityControl>();
            if (cg != null) {
                cg.SetGravity(GetGravityVector());
            }
            gameObject.SetActive(false);
        }
    }

    private Vector3 GetGravityVector() {
        switch (direction) {
            case GravityDirections.Down:
                return Vector3.down * gravForce;
            case GravityDirections.Up:
                return Vector3.up * gravForce;
            case GravityDirections.Left:
                return Vector3.left * gravForce;
            case GravityDirections.Right:
                return Vector3.right * gravForce;
            case GravityDirections.Forward:
                return Vector3.forward * gravForce;
            case GravityDirections.Backward:
                return Vector3.back * gravForce;
            default:
                return Physics.gravity;
        }
    }
}
