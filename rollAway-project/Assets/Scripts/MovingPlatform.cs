using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [Header("Moving Destinations")]
    public GameObject pointA;
    public GameObject pointB;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float delay = 1f;
    public float slowDownThreshold = 2f;

    [SerializeField] GameObject platform;

    private Vector3 targetPosition;

    void Start()
    {
        platform.transform.position = pointA.transform.position;
        targetPosition = pointB.transform.position;
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        while (true)
        {
            float distance = Vector3.Distance(platform.transform.position, targetPosition);

            while (distance > 0.01f)
            {
                float ratio = distance / slowDownThreshold;

                float speedMultiplier = Mathf.Min(ratio, 1f); 

                float currentSpeed = moveSpeed * speedMultiplier;
                float minSpeed = 1f; 

                currentSpeed = Mathf.Max(currentSpeed, minSpeed);

                platform.transform.position = Vector3.MoveTowards(
                    platform.transform.position, 
                    targetPosition, 
                    currentSpeed * Time.deltaTime
                );

                distance = Vector3.Distance(platform.transform.position, targetPosition);
                yield return null;
            }

            platform.transform.position = targetPosition;
            targetPosition = (targetPosition == pointA.transform.position) ? pointB.transform.position : pointA.transform.position;
            
            yield return new WaitForSeconds(delay);
        }
    }
}
