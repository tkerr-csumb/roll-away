using UnityEngine;

public class BowlingBallEffect : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Vector3 spinAxis = Vector3.right;
    [SerializeField] private float spinSpeed = 360f;

    private void Update()
    {
        if (playerController != null && playerController.IsOnIce)
        {
            transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
        }
    }
}