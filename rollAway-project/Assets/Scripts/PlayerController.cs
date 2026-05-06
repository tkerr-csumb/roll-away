using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public enum SurfaceType
    {
        Normal,
        Ice,
        Rubber,
        Sticky,
    }

    private SurfaceType currentSurface = SurfaceType.Normal;

    [Header("References")]
    private Rigidbody rb;
    public GameObject cameraObject;
    private GravityControl gravityControl;
    private BallAudio ballAudio;
    private PlayerDashEnergy dashEnergy;

    [Header("Movement Settings")]
    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float jumpPower = 7f;

    [SerializeField]
    private float dashPower = 5f;

    [SerializeField]
    private float bounceBoost = 10f;

    [SerializeField]
    private float maxSpeed = 11.5f;

    [SerializeField]
    private float currentMoveSpeed;

    [SerializeField]
    private float speedometer;

    [Header("Surface Movement")]
    [SerializeField]
    private float normalLinearDamping = 1f;

    [SerializeField]
    private float normalAngularDamping = 1f;

    [SerializeField]
    private float iceLinearDamping = 0.05f;

    [SerializeField]
    private float iceAngularDamping = 0.05f;

    [SerializeField]
    private float iceAccelerationBoost = 0.1f;

    [SerializeField]
    private float iceSlopeAcceleration = 25f;

    [SerializeField]
    private float iceDownhillMaxSpeed = 50f;
    private Vector3 iceNormal = Vector3.up;
    private bool jumpedFromIce;
    public SurfaceType CurrentSurface => currentSurface;
    public bool IsOnIce => currentSurface == SurfaceType.Ice;

    [SerializeField]
    private float stickyHoldForce = 20f;

    [SerializeField]
    private float stickyMoveMultiplier = 4f;

    [SerializeField]
    private float stickyMaxSpeed = 3.5f;

    [SerializeField]
    private float stickyClimbForce = 14f;
    private static float gravConst = 9.81f;
    private Vector3 stickyNormal = Vector3.zero;

    [Header("Dash & Energy System")]
    public float maxDashEnergy = 100f;
    public float currentDashEnergy = 0f;
    public float energyGainMultiplier = 2f;
    private bool hasBurstCharge = true;
    public bool IsGrounded => isGrounded;

    [Header("Visual Effects")]
    public ParticleSystem dashEffect;
    public GameObject landingVFXPrefab;
    public float impactThreshold = 8.5f;
    private float lastDustTime;
    public float dustCooldown = 0.5f;

    private InputActions inputActions;
    private Vector2 movementInput;
    private bool isGrounded = true;
    private bool onRubber = false;
    private Vector3 lastPosition;

    // public SurfaceType CurrentSurface => currentSurface;
    // public bool IsOnIce => currentSurface == SurfaceType.Ice;
    [NonSerialized]
    public bool onIce = false;
    private bool onSticky = false;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (dashEnergy == null)
            dashEnergy = GetComponent<PlayerDashEnergy>();
        inputActions = new InputActions();
        gravityControl = GetComponent<GravityControl>();
        ballAudio = GetComponent<BallAudio>();
    }

    void OnEnable()
    {
        RollawayInputRemapManager.OnBindingsChanged += RefreshBindings;
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += HandleMoveInput;
        inputActions.Player.Move.canceled += HandleMoveInput;
        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Dash.performed += OnDashPerformed;
    }

    void OnDisable()
    {
        RollawayInputRemapManager.OnBindingsChanged -= RefreshBindings;
        inputActions.Player.Move.performed -= HandleMoveInput;
        inputActions.Player.Move.canceled -= HandleMoveInput;
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Dash.performed -= OnDashPerformed;
        inputActions.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RollawayInputRemapManager.Instance?.ApplyOverridesTo(inputActions.asset);
        currentMoveSpeed = speed;
        lastPosition = transform.position;
        Time.timeScale = 2f;
    }

    void FixedUpdate()
    {
        Vector3 up = GetUpDirection();
        Vector3 move = GetCameraRelativeMove(up);

        ApplySurfaceDamping();
        Vector3 flatVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, up);
        speedometer = flatVelocity.magnitude;
        // Debug.Log($"Current speed: {speedometer}");
        switch (currentSurface)
        {
            case SurfaceType.Sticky:
                HandleStickyMovement(move, up);
                break;
            case SurfaceType.Ice:
                HandleIceMovement(move);
                break;
            default:
                HandleNormalMovement(move);
                break;
        }
    }

    public void HandleMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (!CanJump())
            return;
        ExecuteJump();
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (!CanBurst())
            return;
        ExecuteBurst();
    }

    private Vector3 GetUpDirection()
    {
        if (gravityControl == null)
            return Vector3.up;
        return -gravityControl.GetGravityDirection();
    }

    private Vector3 GetCameraRelativeMove(Vector3 up)
    {
        if (cameraObject == null)
        {
            return new Vector3(movementInput.x, 0f, movementInput.y);
        }

        Vector3 camForward = cameraObject.transform.forward;
        Vector3 camRight = cameraObject.transform.right;

        camForward = Vector3.ProjectOnPlane(camForward, up).normalized;
        camRight = Vector3.ProjectOnPlane(camRight, up).normalized;

        float inputY = movementInput.y;

        // Detect camera pitch
        float pitch = cameraObject.transform.eulerAngles.x;

        // If camera is tilted downward past horizontal, flip vertical input
        bool lookingDown = pitch > 90f && pitch < 270f;
        if (lookingDown)
        {
            inputY *= -1f;
        }

        Vector3 gravDirection = gravityControl.GetGravityDirection().normalized;
        bool gravIsSideways = Mathf.Abs(gravDirection.y) < 0.1f;

        if (!isGrounded && gravIsSideways)
        {
            Vector3 move = Vector3.zero;
            Vector3 flatCamRight = cameraObject.transform.right;
            flatCamRight.y = 0f;
            flatCamRight.Normalize();

            move += flatCamRight * movementInput.x;
            move += Vector3.up * inputY;
            return move;
        }
        return camForward * inputY + camRight * movementInput.x;
    }

    private void ApplySurfaceDamping()
    {
        // If we jumped from ice, keep ice damping while airborne
        if (!isGrounded && jumpedFromIce)
        {
            rb.linearDamping = iceLinearDamping;
            rb.angularDamping = iceAngularDamping;
            return;
        }

        switch (currentSurface)
        {
            case SurfaceType.Ice:
                rb.linearDamping = iceLinearDamping;
                rb.angularDamping = iceAngularDamping;
                break;
            case SurfaceType.Sticky:
                //placeholder in case we want custom damping for flypaper
                // rb.linearDamping = 2f;
                break;
            default:
                rb.linearDamping = normalLinearDamping;
                rb.angularDamping = normalAngularDamping;
                break;
        }
    }

    private void HandleNormalMovement(Vector3 move)
    {
        currentMoveSpeed = speed;
        rb.AddForce(move * currentMoveSpeed, ForceMode.Force);
    }

    private void HandleIceMovement(Vector3 move)
    {
        Vector3 gravityDir = gravityControl.GetGravityDirection();

        // Gravity projected onto the ice surface = downhill direction
        Vector3 downhillDir = Vector3.ProjectOnPlane(gravityDir, iceNormal);

        float slopeAmount = downhillDir.magnitude;

        if (slopeAmount > 0.01f)
        {
            downhillDir.Normalize();

            rb.AddForce(downhillDir * (iceSlopeAcceleration * slopeAmount), ForceMode.Acceleration);
        }

        currentMoveSpeed += iceAccelerationBoost * Time.fixedDeltaTime;
        currentMoveSpeed = Mathf.Min(currentMoveSpeed, maxSpeed);

        rb.AddForce(move * currentMoveSpeed, ForceMode.Force);

        float speedCap = Mathf.Lerp(maxSpeed, iceDownhillMaxSpeed, slopeAmount);

        if (rb.linearVelocity.magnitude > speedCap)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, speedCap);
        }
    }

    private void HandleStickyMovement(Vector3 move, Vector3 up)
    {
        bool onWall = Mathf.Abs(Vector3.Dot(stickyNormal, up)) < 0.5f;
        var keyboard = Keyboard.current;
        if (onWall)
        {
            Vector3 climbingMovement = Vector3.ProjectOnPlane(move, stickyNormal);
            //keep ball attached and not fall off
            rb.AddForce(-stickyNormal * stickyHoldForce, ForceMode.Force);
            // offset gravity while climbing (not all the way though)
            if (movementInput.y > 0.1f)
            {
                rb.AddForce(up * stickyClimbForce, ForceMode.Force);
            }
            else if (movementInput.y < -0.1f)
            {
                rb.AddForce(-up * stickyClimbForce, ForceMode.Force);
            }
            else
            {
                rb.AddForce(up * gravConst, ForceMode.Force);
            }

            rb.AddForce(climbingMovement * (speed * stickyMoveMultiplier), ForceMode.Force);
            Vector3 wallVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, stickyNormal);
            Vector3 intoWallVelocity = rb.linearVelocity - wallVelocity;

            if (wallVelocity.magnitude > stickyMaxSpeed)
            {
                wallVelocity = Vector3.ClampMagnitude(wallVelocity, stickyMaxSpeed);
                rb.linearVelocity = wallVelocity + intoWallVelocity;
            }
            return;
        }
    }

    private bool CanJump()
    {
        return isGrounded;
    }

    private bool CanBurst()
    {
        return hasBurstCharge && dashEnergy != null & dashEnergy.HasEnergy();
    }

    void ExecuteJump()
    {
        Vector3 up = GetUpDirection();

        if (currentSurface == SurfaceType.Ice)
        {
            return;
        }
        else if (currentSurface == SurfaceType.Rubber)
        {
            jumpedFromIce = false;
            rb.AddForce(up * bounceBoost, ForceMode.Impulse);
        }
        else
        {
            jumpedFromIce = false;
            rb.AddForce(up * jumpPower, ForceMode.Impulse);
        }

        ballAudio?.PlayJump();
        isGrounded = false;
    }

    void ExecuteBurst()
    {
        Vector3 gravityDir = gravityControl.GetGravityDirection();
        Vector3 up = GetUpDirection();
        Vector3 dashDirection = GetCameraRelativeMove(up).normalized;

        if (dashDirection.sqrMagnitude < 0.01f)
            dashDirection = transform.forward;
        rb.AddForce(dashDirection * dashPower, ForceMode.Impulse);
        ballAudio?.PlayDash();
        hasBurstCharge = false;
        if (dashEnergy != null)
        {
            dashEnergy.ConsumeAlleEnergy();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleEnemyCollision(collision);
        HandleSurfaceEnter(collision);
        HandleGroundingOnEnter(collision);
        TriggerLandingVFX(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        UpdateGroundedState(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        HandleSurfaceExit(collision);
    }

    private void HandleEnemyCollision(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
            return;
        Destroy(gameObject);
        // need to add losing state here
    }

    private void HandleSurfaceEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            currentSurface = SurfaceType.Ice;
            iceNormal = collision.contacts[0].normal;
        }
        else if (collision.gameObject.CompareTag("Rubber"))
        {
            currentSurface = SurfaceType.Rubber;
        }
        else if (collision.gameObject.CompareTag("Flypaper"))
        {
            currentSurface = SurfaceType.Sticky;
            stickyNormal = collision.contacts[0].normal;
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            currentSurface = SurfaceType.Normal;
        }
    }

    private void HandleSurfaceExit(Collision collision)
    {
        if (
            collision.gameObject.CompareTag("Ice")
            || collision.gameObject.CompareTag("Rubber")
            || collision.gameObject.CompareTag("Flypaper")
        )
        {
            currentSurface = SurfaceType.Normal;
            stickyNormal = Vector3.zero;
        }
    }

    private bool IsFloorNormal(Vector3 normal)
    {
        Vector3 up = GetUpDirection();
        return Vector3.Dot(normal, up) > 0.5f;
    }

    private void HandleGroundingOnEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;

        if (IsFloorNormal(normal))
        {
            isGrounded = true;
            hasBurstCharge = true;
            jumpedFromIce = false;
        }

        if (currentSurface == SurfaceType.Sticky)
        {
            stickyNormal = normal;
        }
    }

    private void UpdateGroundedState(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;

        if (IsFloorNormal(normal))
        {
            isGrounded = true;
        }

        if (currentSurface == SurfaceType.Sticky)
        {
            stickyNormal = normal;
        }

        if (currentSurface == SurfaceType.Ice)
        {
            iceNormal = collision.contacts[0].normal;
        }
    }

    private void TriggerLandingVFX(Collision collision)
    {
        if (
            collision.relativeVelocity.magnitude > impactThreshold
            && Time.time > lastDustTime + dustCooldown
        )
        {
            if (landingVFXPrefab != null)
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 spawnPos = contact.point + Vector3.up * 0.02f;
                Instantiate(landingVFXPrefab, spawnPos, Quaternion.identity);
                lastDustTime = Time.time;
            }
        }
    }

    private void RefreshBindings()
    {
        RollawayInputRemapManager.Instance?.ApplyOverridesTo(inputActions.asset);
    }
}
