using UnityEngine;

public class GoalArrow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform goal;
    [SerializeField] private GravityControl gravityControl;

    [Header("Positioning")]
    [SerializeField] private float heightOffset = 2f;
    [SerializeField] private float rotateSpeed  = 8f;

    void Update()
    {
        if (player == null || goal == null) return;

        Vector3 up = gravityControl != null
            ? -gravityControl.GetGravityDirection()
            : Vector3.up;

        transform.position = player.position + up * heightOffset;

        Vector3 toGoal = goal.position - transform.position;
        if (toGoal.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(toGoal, up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
}