using System;
using UnityEngine;

public class GravityArrowPrefab : MonoBehaviour {
    public enum GravityDirections { Up, Down, Left, Right, Forward, Backward }

    [SerializeField] private GravityDirections direction;

    private float gravForce = 9.81f;
    private float itemTimerMax = 6f;
    private float itemTimer;

    private Renderer[] renderers;
    private bool isActive = true;

    private void Awake() {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update() {
        if (!isActive) {
            itemTimer += Time.deltaTime;

            if (itemTimer >= itemTimerMax) {
                SetVisualState(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!isActive) return;

        if (other.CompareTag("Player")) {
            GravityControl cg = other.GetComponent<GravityControl>();
            if (cg != null) {
                cg.SetGravity(GetGravityVector());
            }

            SetVisualState(false);
            itemTimer = 0f;
        }
    }

    private void SetVisualState(bool state) {
        isActive = state;

        foreach (var rend in renderers)
            rend.enabled = state;
    }

    private Vector3 GetGravityVector() {
        switch (direction) {
            case GravityDirections.Down:
                return Vector3.down * gravForce;
            case GravityDirections.Up:
                return Vector3.up * gravForce;
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