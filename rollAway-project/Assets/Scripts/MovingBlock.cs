using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float amplitude;

    [SerializeField]
    private float offset;
    private Rigidbody rigidbody;

    [SerializeField]
    private Transform center;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float newX = Mathf.Cos(offset + Time.time * speed) * amplitude + center.position.x;
        rigidbody.MovePosition(new Vector3(newX, rigidbody.position.y, rigidbody.position.z));
    }
}
