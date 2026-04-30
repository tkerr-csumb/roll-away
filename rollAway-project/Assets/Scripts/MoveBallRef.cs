using UnityEngine;

public class MoveBallRef : MonoBehaviour
{
    Transform parentTransform;

    void Start()
    {
        parentTransform = transform.parent;
    }

    void Update()
    {
        if (parentTransform != null)
        {
            transform.position = parentTransform.position;
        }
    }
}
