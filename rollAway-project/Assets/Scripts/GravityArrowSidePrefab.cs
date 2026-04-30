using System;
using UnityEngine;

public class GravityArrowSidePrefab : MonoBehaviour
{
    public enum GravityDirections
    {
        Left,
        Right,
        Forward,
        Backward,
    }

    [SerializeField]
    private GravityDirections direction;

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
            Vector3 targetGravity = GetGravityVector();

            GravityControl gravityControl = other.GetComponent<GravityControl>();
            if (gravityControl != null)
            {
                float gravityAlignment = Vector3.Dot(
                    gravityControl.GetGravityDirection(),
                    targetGravity.normalized
                );
                if (gravityAlignment > 0.99f)
                {
                    return;
                }

                gravityControl.SetGravity(targetGravity);
            }
            else
            {
                SideGravityControl cg = other.GetComponent<SideGravityControl>();
                if (cg != null)
                {
                    if (Vector3.Dot(cg.GetGravityDirection(), targetGravity.normalized) > 0.99f)
                    {
                        return;
                    }

                    cg.SetGravity(targetGravity);
                }
                else
                {
                    return;
                }
            }

            SetVisualState(false);
            itemTimer = 0f;
        }
    }

    private void SetVisualState(bool state)
    {
        isActive = state;

        foreach (var rend in renderers)
            rend.enabled = state;
    }

    private Vector3 GetGravityVector()
    {
        switch (direction)
        {
            case GravityDirections.Left:
                return Vector3.left * gravForce;
            case GravityDirections.Right:
                return Vector3.right * gravForce;
            case GravityDirections.Forward:
                return Vector3.forward * gravForce;
            case GravityDirections.Backward:
                return Vector3.back * gravForce;
            default:
                return Physics.gravity;
        }
    }
}
