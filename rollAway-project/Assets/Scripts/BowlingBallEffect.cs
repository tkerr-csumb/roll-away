using UnityEngine;

public class BowlingBallEffect : MonoBehaviour
{
    public PlayerController playerController;
    private Vector3 spinAxis = Vector3.right;
    private float spinSpeed = 360f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.onIce)
        {
            transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
            // Debug.Log("Woah slidey");
        }
    }
}
