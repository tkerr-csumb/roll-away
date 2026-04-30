using UnityEngine;

public class ObstacleKnockback : MonoBehaviour
{
    // Hit the player to the left with this force
    private float knockbackForce = 50f;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Break();
    }
}
