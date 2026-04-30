using UnityEngine;

public class GravityArrowPrefab : MonoBehaviour {
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

        if (other.CompareTag("Player")){
            GravityControl cg = other.GetComponent<GravityControl>();
            if (cg != null) {
                cg.InvertGravity();
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
}