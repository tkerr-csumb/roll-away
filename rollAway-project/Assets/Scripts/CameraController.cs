using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float distance = 7f;
    public float alignmentSpeed = 10f;

    private GravityControl gravityControl;

    private Vector3 thisSideUp;
    private Vector3 followVelocity;

    void Start()
    {
        gravityControl = player.GetComponent<GravityControl>();
        thisSideUp = Vector3.up;


    }

    void LateUpdate() {
        Vector3 targetUp = -gravityControl.GetGravityDirection();
        thisSideUp = Vector3.Slerp(thisSideUp, targetUp, alignmentSpeed * Time.deltaTime);

        Vector3 forward = player.transform.forward;

        forward = Vector3.ProjectOnPlane(forward, thisSideUp).normalized;

        transform.rotation = Quaternion.LookRotation(forward, thisSideUp);

        Vector3 desiredPosition = player.transform.position - transform.forward * distance;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            0.12f
        );
    }
}