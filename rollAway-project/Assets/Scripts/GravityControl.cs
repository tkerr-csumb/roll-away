using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GravityControl : MonoBehaviour {
    private Rigidbody rb;
    private Vector3 currentGravity;

    [Header("Cinemachine")]
    public CinemachineCamera virtualCamera;

    [Header("Flip Settings")]
    public float flipDuration = 1.75f;

    private bool isFlipping = false;
    public static event System.Action OnGravityFlipped;
    public CinemachineOrbitalFollow orbitalFollow;
    private bool isUpsideDown;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        currentGravity = Physics.gravity;
    }

    void FixedUpdate() {
        rb.AddForce(currentGravity, ForceMode.Acceleration);
    }

    public void InvertGravity() {
        currentGravity = -currentGravity;
        isUpsideDown = !isUpsideDown;
        OnGravityFlipped?.Invoke();

        ApplyVerticalAxisRange();
        if (virtualCamera != null) {
            StartCoroutine(SmoothDutchFlip());
        }
    }

    private IEnumerator SmoothDutchFlip() {
        isFlipping = true;

        float startDutch = virtualCamera.Lens.Dutch;
        float targetDutch = startDutch + 180f;
        float time = 0f;

        var inputController = virtualCamera.GetComponent<CinemachineInputAxisController>();

        if (inputController != null) {
            foreach (var controller in inputController.Controllers) {
                if (controller.Input != null) {
                    controller.Input.Gain *= -1f;
                }
            }
        }

        while (time < flipDuration) {
            time += Time.deltaTime;
            float t = time / flipDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            var lens = virtualCamera.Lens;
            lens.Dutch = Mathf.Lerp(startDutch, targetDutch, smoothT);
            virtualCamera.Lens = lens;

            yield return null;
        }

        var finalLens = virtualCamera.Lens;
        finalLens.Dutch = targetDutch % 360f;
        virtualCamera.Lens = finalLens;

        isFlipping = false;
    }
    private void ApplyVerticalAxisRange() {
        if (orbitalFollow == null)
            return;

        if (!isUpsideDown) {
            orbitalFollow.VerticalAxis.Range = new Vector2(0f, 45f);
            orbitalFollow.VerticalAxis.Center = 22.5f;
        } else {
            orbitalFollow.VerticalAxis.Range = new Vector2(-45f, 0f);
            orbitalFollow.VerticalAxis.Center = -22.5f;
        }
    }

    public Vector3 GetGravityDirection() {
        return currentGravity.normalized;
    }
}