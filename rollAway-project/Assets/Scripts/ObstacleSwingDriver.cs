using UnityEngine;

public class ObstacleSwingDriver : MonoBehaviour
{
    private Rigidbody rb;
    private HingeJoint hinge;

    public float startingAngle = 0f;

    [Header("Speed Settings")]
    public float swingSpeed = 4.05f;
    public float centerBoost = 1.875f;

    private int direction = 1;

    // void Awake()
    // {
    //     rb = GetComponent<Rigidbody>();
    //     hinge = GetComponent<HingeJoint>();

    //     transform.localRotation = Quaternion.AngleAxis(startingAngle, hinge.axis);
    // }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();

        Quaternion spawnRotation = Quaternion.AngleAxis(startingAngle, hinge.axis);
        
        rb.position = transform.position; 
        rb.rotation = transform.rotation * spawnRotation;

        direction = (startingAngle >= 0) ? -1 : 1;
    }

    void FixedUpdate()
    {
        if (rb == null || hinge == null)
        {
            return;
        }

        float currentAngle = hinge.angle;

        if (float.IsNaN(currentAngle) || float.IsInfinity(currentAngle))
        {
            return;
        }

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

        Vector3 worldAxis = transform.TransformDirection(hinge.axis.normalized);
        rb.angularVelocity = worldAxis * targetVelocity;
    }
}
