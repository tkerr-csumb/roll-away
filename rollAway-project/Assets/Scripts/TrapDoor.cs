using System.Collections;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    [Header("Settings")]
    public float delayBeforeDrop = 1.0f;
    public float shakeIntensity = 0.05f;
    public Vector3 openAngleOffset = new Vector3(-90, 0, 0);

    private Vector3 originalChildPos;
    private Quaternion closedRotation;
    private bool isTriggered = false;
    private Transform meshChild;

    void Start()
    {
        closedRotation = transform.localRotation;
        // Grab the door mesh
        meshChild = transform.GetChild(0);
        originalChildPos = meshChild.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            StartCoroutine(ShakeAndDrop());
        }
    }
    public void ResetDoor()
    {
        // Stop it from rotating/dropping
        transform.localRotation = closedRotation;

        // Make sure the mesh is back in its original spot
        if (meshChild != null)
        {
            meshChild.localPosition = originalChildPos;
        }

        // Allow the player to trigger it again
        isTriggered = false;
        
        Debug.Log("Trap Door Reset");
    }
    IEnumerator ShakeAndDrop()
    {
        isTriggered = true;
        float elapsed = 0f;

        // Shake effect
        while (elapsed < delayBeforeDrop)
        {
            // Calculate a random offset
            float xOffset = Random.Range(-1f, 1f) * shakeIntensity;
            float zOffset = Random.Range(-1f, 1f) * shakeIntensity;

            // Apply the offset to the mesh's local position
            meshChild.localPosition = new Vector3(originalChildPos.x + xOffset, originalChildPos.y, originalChildPos.z + zOffset);

            elapsed += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Reset child position before dropping
        meshChild.localPosition = originalChildPos;

        // Drops the door by rotating it open
        transform.localRotation = closedRotation * Quaternion.Euler(openAngleOffset);
        
        // Resets after a few seconds
        yield return new WaitForSeconds(3f);
        ResetDoor();
    }
}