using UnityEngine;

public class ObstacleSwingDriver : MonoBehaviour
{
    private Rigidbody rb;
    private HingeJoint hinge;

    [Header("Speed Settings")]
    public float swingSpeed = 1.85f;
    public float centerBoost = 1.75f;

    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();
        
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        float currentAngle = hinge.angle;

        if (direction == 1 && currentAngle >= hinge.limits.max - 0.5f)
        {
            direction = -1;
        }
        else if (direction == -1 && currentAngle <= hinge.limits.min + 0.5f)
        {
            direction = 1;
        }

        float speedFactor = Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float targetVelocity = direction * swingSpeed * (speedFactor * centerBoost);

        rb.angularVelocity = transform.TransformDirection(Vector3.forward) * targetVelocity;
    }
}
