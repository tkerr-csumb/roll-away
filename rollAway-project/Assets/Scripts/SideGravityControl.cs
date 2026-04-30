using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SideGravityControl : MonoBehaviour
{
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
        currentGravity = newGravity;
    }

    public Vector3 GetGravityDirection()
    {
        return currentGravity.normalized;
    }
}
