using System.Collections;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    [Header("Timing")]
    public float fallDelay = 2.0f;      // How long the player can stand on it
    public float resetDelay = 3.0f;     // How long before it closes back up

    [Header("Rotation Settings")]
    public float openAngle = -90f;      // -90 swings it downward
    
    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private BoxCollider platformCollider;
    private bool isTriggered = false;

    void Start()
    {
        // Get the collider from the child mesh so we can disable it
        platformCollider = GetComponentInChildren<BoxCollider>();
        
        // Store the starting rotation
        closedRotation = transform.localRotation;
        
        // Calculate the "Open" rotation (Rotating around the Z-axis hinge)
        openedRotation = closedRotation * Quaternion.Euler(0, 0, openAngle);
    }

    private void OnCollisionEnter(Collision collision)
    {   
        Debug.Log("Something hit me: " + collision.gameObject.name); 
        // Check if it's the player and the trap isn't already running
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            StartCoroutine(TrapRoutine());
        }
    }

    IEnumerator TrapRoutine()
    {
        isTriggered = true;

        // Give the player a 2-second warning
        yield return new WaitForSeconds(fallDelay);

        // Open the hinge and disable the floor collider so they fall
        transform.parent.localRotation = openedRotation;
        if (platformCollider != null) platformCollider.enabled = false;

        // Keep it open for a few seconds
        yield return new WaitForSeconds(resetDelay);

        // Reset the door to its original position
        transform.parent.localRotation = closedRotation;
        if (platformCollider != null) platformCollider.enabled = true;
        
        isTriggered = false;
    }
}