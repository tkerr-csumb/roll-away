using UnityEngine;

public class MoveBallRef : MonoBehaviour
{
    Rigidbody rb;
    Transform parentTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        parentTransform = transform.parent;
    }

    void FixedUpdate()
    {
        if (rb != null && parentTransform != null)
        {
            rb.MovePosition(parentTransform.position);
        }
    }
}
