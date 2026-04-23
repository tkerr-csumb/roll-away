using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float alignSpeed = 5f;
    public float sensitivity = 2f;
    public float followSpeed = 15f;

    private GravityControl gravityControl;
    private Vector3 currentUp;
    private float yaw, pitch;

    void Start()
    {
        gravityControl = player.GetComponent<GravityControl>();
        currentUp = -gravityControl.GetGravityDirection();
        transform.position = player.position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, followSpeed * Time.deltaTime);

        Vector3 targetUp = -gravityControl.GetGravityDirection();
        currentUp = Vector3.Slerp(currentUp, targetUp, alignSpeed * Time.deltaTime);

        Vector2 mouse = Mouse.current.delta.ReadValue();
        float mouseScale = sensitivity * 0.02f;

        yaw += mouse.x * mouseScale;
        pitch -= mouse.y * mouseScale;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        Quaternion yawRot = Quaternion.AngleAxis(yaw, currentUp);
        Vector3 right = yawRot * Vector3.right;
        Vector3 forward = Quaternion.AngleAxis(pitch, right) * (yawRot * Vector3.forward);

        transform.rotation = Quaternion.LookRotation(forward, currentUp);
    }
}