using UnityEngine;

public class GravityArrowPrefab : MonoBehaviour
{
    public enum GravityDirection
    {
        Up = 0,
        Down = 1,
    }

    [SerializeField]
    private GravityDirection direction = GravityDirection.Down;

    private float gravForce = 9.81f;

    private float itemTimerMax = 6f;
    private float itemTimer;

    private Renderer[] renderers;
    private bool isActive = true;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (!isActive)
        {
            itemTimer += Time.deltaTime;

            if (itemTimer >= itemTimerMax)
            {
                SetVisualState(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;

        if (other.CompareTag("Player"))
        {
            GravityControl cg = other.GetComponent<GravityControl>();
            if (cg != null)
            {
                Vector3 targetGravity = GetTargetGravity();
                if (Vector3.Dot(cg.GetGravityDirection(), targetGravity.normalized) > 0.99f)
                {
                    return;
                }

                cg.SetGravity(targetGravity, true);

                SetVisualState(false);
                itemTimer = 0f;
                return;
            }

            SideGravityControl sideGravity = other.GetComponent<SideGravityControl>();
            if (sideGravity != null)
            {
                Vector3 targetGravity = GetTargetGravity();
                if (
                    Vector3.Dot(sideGravity.GetGravityDirection(), targetGravity.normalized) > 0.99f
                )
                {
                    return;
                }

                sideGravity.SetGravity(targetGravity);

                SetVisualState(false);
                itemTimer = 0f;
            }
        }
    }

    private void SetVisualState(bool state)
    {
        isActive = state;

        foreach (var rend in renderers)
            rend.enabled = state;
    }

    private Vector3 GetTargetGravity()
    {
        switch (direction)
        {
            case GravityDirection.Up:
                return Vector3.up * gravForce;
            case GravityDirection.Down:
            default:
                return Vector3.down * gravForce;
        }
    }
}
