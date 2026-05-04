using UnityEngine;

public class GoalArrow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform goal;
    private Camera gameCamera;
    private RectTransform arrowRect;

    void Awake()
    {
        arrowRect = GetComponent<RectTransform>();
        gameCamera = Camera.main;
    }
    void Update()
    {
        if (player == null || goal == null || gameCamera == null) return;

        Vector3 toGoal = goal.position - player.position;

        float x = Vector3.Dot(toGoal, gameCamera.transform.right);
        float y = Vector3.Dot(toGoal, gameCamera.transform.up);

        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg - 90f;
        arrowRect.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}