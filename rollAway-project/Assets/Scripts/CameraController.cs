using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float sensitivity = 3f;
    public float distance = 7f;
    private GravityControl gravityControl;
    private Vector3 thisSideUp;
    public float alignmentSpeed = 10f;

    void Start() {
        gravityControl = player.GetComponent<GravityControl>();
        thisSideUp = Vector3.up;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate() {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        Vector3 gravityDir = gravityControl.GetGravityDirection();
        Vector3 targetUp = -gravityDir;
        thisSideUp = Vector3.Slerp(thisSideUp, targetUp, alignmentSpeed * Time.deltaTime);

        Quaternion currentRotation = transform.rotation;

        Quaternion yawRotation = Quaternion.AngleAxis(
            mouseDelta.x * sensitivity,
            thisSideUp
        );

        Vector3 right = currentRotation * Vector3.right;

        Quaternion pitchRotation = Quaternion.AngleAxis(
            -mouseDelta.y * sensitivity,
            right
        );

        Quaternion targetRotation = yawRotation * pitchRotation * currentRotation;

        Vector3 forward = targetRotation * Vector3.forward;
        float angleFromUp = Vector3.Angle(forward, thisSideUp);

        if (angleFromUp < 10f || angleFromUp > 170f) {
            targetRotation = yawRotation * currentRotation;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            15f * Time.deltaTime
        );

        transform.position = player.transform.position - transform.forward * distance;
    }
}