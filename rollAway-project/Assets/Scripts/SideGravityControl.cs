using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SideGravityControl : MonoBehaviour
{
    public static event System.Action OnGravityFlipped;

    private Rigidbody rb;
    private Vector3 currentGravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        currentGravity = Physics.gravity;
    }

    void FixedUpdate()
    {
        rb.AddForce(currentGravity, ForceMode.Acceleration);
    }

    public void SetGravity(Vector3 newGravity)
    {
        if (Vector3.Dot(currentGravity.normalized, newGravity.normalized) < 0.9f)
        {
            OnGravityFlipped?.Invoke();
        }
        currentGravity = newGravity;
    }

    public Vector3 GetGravityDirection()
    {
        return currentGravity.normalized;
    }
}
