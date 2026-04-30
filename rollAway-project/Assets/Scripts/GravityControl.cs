using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityControl : MonoBehaviour
{
    public static event System.Action OnGravityFlipped;

    private Rigidbody rb;
    private Vector3 currentGravity;

    [Header("Cinemachine")]
    public CinemachineCamera virtualCamera;

    [Header("Flip Settings")]
    public float flipDuration = 1.75f;

    private bool isFlipping = false;
    private Coroutine dutchFlipCoroutine;
    public CinemachineOrbitalFollow orbitalFollow;
    private bool isUpsideDown;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        currentGravity = Physics.gravity;
    }

    void FixedUpdate()
    {
        rb.AddForce(currentGravity, ForceMode.Acceleration);
    }

    public void SetGravity(Vector3 newGravity, bool animateCameraFlip = false)
    {
        if (newGravity.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        bool previousUpsideDown = isUpsideDown;
        Vector3 previousGravity = currentGravity;
        currentGravity = newGravity;

        bool currentIsVertical = Mathf.Abs(currentGravity.normalized.y) > 0.9f;
        bool previousIsVertical = Mathf.Abs(previousGravity.normalized.y) > 0.9f;

        if (currentIsVertical)
        {
            isUpsideDown = Vector3.Dot(currentGravity.normalized, Physics.gravity.normalized) < 0f;
            ApplyVerticalAxisRange();

            if (animateCameraFlip && virtualCamera != null)
            {
                float targetDutch = isUpsideDown ? 180f : 0f;
                bool invertInputGains = previousUpsideDown != isUpsideDown;
                StartDutchTransition(targetDutch, invertInputGains);
            }
        }
        else
        {
            // Keep camera orientation while sideways and only widen the lower/upper
            // edge by 10 degrees until gravity becomes vertical again.
            ApplySidewaysVerticalAxisRange();
        }

        // Fire event if gravity direction meaningfully changed
        if (Vector3.Dot(previousGravity.normalized, currentGravity.normalized) < 0.9f)
        {
            OnGravityFlipped?.Invoke();
        }
    }

    public void InvertGravity()
    {
        SetGravity(-currentGravity, true);
    }

    private void StartDutchTransition(float targetDutch, bool invertInputGains)
    {
        if (dutchFlipCoroutine != null)
        {
            StopCoroutine(dutchFlipCoroutine);
        }

        dutchFlipCoroutine = StartCoroutine(SmoothDutchTo(targetDutch, invertInputGains));
    }

    private IEnumerator SmoothDutchTo(float targetDutch, bool invertInputGains)
    {
        isFlipping = true;

        float startDutch = virtualCamera.Lens.Dutch;
        float time = 0f;

        var inputController = virtualCamera.GetComponent<CinemachineInputAxisController>();

        if (invertInputGains && inputController != null)
        {
            foreach (var controller in inputController.Controllers)
            {
                if (controller.Input != null)
                {
                    controller.Input.Gain *= -1f;
                }
            }
        }

        while (time < flipDuration)
        {
            time += Time.deltaTime;
            float t = time / flipDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            var lens = virtualCamera.Lens;
            lens.Dutch = Mathf.LerpAngle(startDutch, targetDutch, smoothT);
            virtualCamera.Lens = lens;

            yield return null;
        }

        var finalLens = virtualCamera.Lens;
        finalLens.Dutch = Mathf.Repeat(targetDutch, 360f);
        virtualCamera.Lens = finalLens;

        isFlipping = false;
        dutchFlipCoroutine = null;
    }

    private void ApplyVerticalAxisRange()
    {
        if (orbitalFollow == null)
            return;

        if (!isUpsideDown)
        {
            orbitalFollow.VerticalAxis.Range = new Vector2(0f, 45f);
            orbitalFollow.VerticalAxis.Center = 22.5f;
        }
        else
        {
            orbitalFollow.VerticalAxis.Range = new Vector2(-45f, 0f);
            orbitalFollow.VerticalAxis.Center = -22.5f;
        }
    }

    private void ApplySidewaysVerticalAxisRange()
    {
        if (orbitalFollow == null)
            return;

        if (!isUpsideDown)
        {
            orbitalFollow.VerticalAxis.Range = new Vector2(-10f, 45f);
            orbitalFollow.VerticalAxis.Center = 17.5f;
        }
        else
        {
            orbitalFollow.VerticalAxis.Range = new Vector2(-45f, 10f);
            orbitalFollow.VerticalAxis.Center = -17.5f;
        }
    }

    public Vector3 GetGravityDirection()
    {
        return currentGravity.normalized;
    }

    public Vector3 GetGravityVector()
    {
        return currentGravity;
    }
}
