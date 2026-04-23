using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float sensitivity = 2f;
    public float distance = 7f;
    public float alignmentSpeed = 10f;
    public float mouseSmoothing = 0.08f;

    private GravityControl gravityControl;

    private Vector3 thisSideUp;

    private float yaw;
    private float pitch;

    private Vector2 smoothedMouse;
    private Vector2 mouseVelocity;

    private Vector3 followVelocity;

    void Start()
    {
        gravityControl = player.GetComponent<GravityControl>();
        thisSideUp = Vector3.up;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        smoothedMouse = Vector2.SmoothDamp(
            smoothedMouse,
            mouseDelta,
            ref mouseVelocity,
            mouseSmoothing
        );

        Vector3 targetUp = -gravityControl.GetGravityDirection();

        thisSideUp = Vector3.Slerp(thisSideUp, targetUp, alignmentSpeed * Time.deltaTime);

        float mouseScale = sensitivity * 0.02f;

        yaw += smoothedMouse.x * mouseScale;
        pitch -= smoothedMouse.y * mouseScale;

        pitch = Mathf.Clamp(pitch, -80f, 80f);

        Quaternion yawRot = Quaternion.AngleAxis(yaw, thisSideUp);

        Vector3 right = yawRot * Vector3.right;

        Vector3 forward = Quaternion.AngleAxis(pitch, right) * (yawRot * Vector3.forward);

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
