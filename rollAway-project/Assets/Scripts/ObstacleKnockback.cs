using UnityEngine;

public class ObstacleKnockback : MonoBehaviour
{
    private float knockbackForce = 10f;
    private float upwardBias = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            Vector3 contactPoint = collision.contacts[0].point;
            ApplyKnockback(player, contactPoint);
        }
    }

    private void ApplyKnockback(PlayerController player, Vector3 contactPoint)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            Vector3 knockbackDirection = (player.transform.position - contactPoint).normalized;
            knockbackDirection += Vector3.up * upwardBias;
            knockbackDirection = knockbackDirection.normalized;
            
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }
}
