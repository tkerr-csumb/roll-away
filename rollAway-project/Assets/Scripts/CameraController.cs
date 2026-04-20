using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float sensitivity = 3f;
    public float distance = 5f;

    private float yaw;
    private float pitch;

    private GravityControl gravityControl;

    void Start() {
        gravityControl = player.GetComponent<GravityControl>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate() {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * sensitivity * Time.deltaTime;
        pitch -= mouseDelta.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        Vector3 gravityDir = gravityControl.GetGravityDirection();
        Vector3 up = -gravityDir;

        Quaternion rotation = Quaternion.LookRotation(
            Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward,
            up
        );

        transform.position = player.transform.position - rotation * Vector3.forward * distance;
        transform.rotation = rotation;
    }
}