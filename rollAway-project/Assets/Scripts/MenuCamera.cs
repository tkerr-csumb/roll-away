using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [SerializeField] Transform ball;
    [SerializeField] Vector3 offset = new Vector3(-8, 4, 0);
    [SerializeField] float smoothSpeed = 3f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void LateUpdate()
    {
        Vector3 targetPos = ball.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
        transform.LookAt(ball);
    }
}