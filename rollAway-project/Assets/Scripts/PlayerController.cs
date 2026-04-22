using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    public float speed = 10f;
    public float jumpPower = 7f;
    public float dashPower = 5f;
    public float bounceBoost = 10f;

    [Header("Dash & Energy System")]
    public float maxDashEnergy = 100f;
    public float currentDashEnergy = 0f;
    public float energyGainMultiplier = 2f;

    [Header("Visual Effects")]
    public ParticleSystem dashEffect;
    public GameObject landingVFXPrefab;
    public float impactThreshold = 8.5f;

    [Header("UI References")]
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    [Header("Internal State")]
    private Rigidbody rb;
    private bool hasBurstCharge = true;
    private Vector2 movementInput;
    private int count;
    private bool isGrounded = true;
    public bool onIce = false;
    private bool onRubber = false;
    private Vector3 lastPosition;
    private InputActions inputActions;
    public GameObject cameraObject;
    private GravityControl gravityControl;

    void Awake()
    {
        Instance = this;
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Move.performed += HandleMoveInput;
        inputActions.Player.Move.canceled += HandleMoveInput;

        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Dash.performed += OnDashPerformed;

        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= HandleMoveInput;
        inputActions.Player.Move.canceled -= HandleMoveInput;
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Dash.performed -= OnDashPerformed;
        inputActions.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastPosition = transform.position;

        rb = GetComponent<Rigidbody>();
        gravityControl = GetComponent<GravityControl>();

        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    void Update()
    {
        // Calculate the distance covered (ignoring vertical movement)
        Vector3 currentPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 lastPosFlat = new Vector3(lastPosition.x, 0, lastPosition.z);
        float distanceMoved = Vector3.Distance(currentPosFlat, lastPosFlat);

        // Only add energy if touching the ground
        if (isGrounded && currentDashEnergy < maxDashEnergy)
        {
            currentDashEnergy += distanceMoved * energyGainMultiplier;

            // Keep it from going over the max
            currentDashEnergy = Mathf.Clamp(currentDashEnergy, 0, maxDashEnergy);
        }

        lastPosition = transform.position;
    }

    public void HandleMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            if (!onRubber)
            {
                Vector3 gravityDir = gravityControl.GetGravityDirection();
                Vector3 up = -gravityDir;

                rb.AddForce(up * jumpPower, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(Vector3.up * bounceBoost, ForceMode.Impulse);
            }
            isGrounded = false;
        }
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        // Check if the bar is full enough to dash
        if (currentDashEnergy >= maxDashEnergy)
        {
            ExecuteBurst();
            // Reset the energy so the bar empties
            currentDashEnergy = 0f;
        }
    }

    void ExecuteBurst()
    {
        Vector3 gravityDir = gravityControl.GetGravityDirection();
        Vector3 up = -gravityDir;

        Vector3 camForward = cameraObject.transform.forward;
        Vector3 camRight = cameraObject.transform.right;

        camForward = Vector3.ProjectOnPlane(camForward, up).normalized;
        camRight = Vector3.ProjectOnPlane(camRight, up).normalized;

        // This resets the velocity along the up axis, allowing for a consistent dash
        rb.linearVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, up);

        // The forward dash logic (camera-relative input)
        Vector3 dashDirection = camForward * movementInput.y + camRight * movementInput.x;

        // If there's no input, we can default to the current facing direction or forward
        if (dashDirection == Vector3.zero)
            dashDirection = Vector3.ProjectOnPlane(cameraObject.transform.forward, up).normalized;
        if (dashDirection == Vector3.zero)
            dashDirection = camForward;

        // Apply the forces for the dash
        rb.AddForce(dashDirection * dashPower, ForceMode.VelocityChange);

        // Adding slight upward lift (relative to gravity)
        rb.AddForce(up * 3f, ForceMode.VelocityChange);

        if (dashEffect != null)
        {
            dashEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            dashEffect.Play();
        }

        // sound would go here
    }

    void SetCountText()
    {
        countText.text = "Polyhedrons: " + count.ToString();
        if (count >= 12)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    void FixedUpdate()
    {
        Vector3 gravityDir = gravityControl.GetGravityDirection();
        Vector3 up = -gravityDir;

        Vector3 camForward = cameraObject.transform.forward;
        Vector3 camRight = cameraObject.transform.right;

        camForward = Vector3.ProjectOnPlane(camForward, up).normalized;
        camRight = Vector3.ProjectOnPlane(camRight, up).normalized;

        Vector3 move = camForward * movementInput.y + camRight * movementInput.x;
        if (onIce)
        {
            rb.linearDamping = 0.05f; // very low = slidey
        }
        else
        {
            rb.linearDamping = 1f; // normal
        }
        rb.AddForce(move * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleGroundCollision(collision);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text =
                "HAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHA";
        }
    }

    private void HandleGroundCollision(Collision collision)
    {
        if (
            collision.gameObject.CompareTag("Ground")
            || collision.gameObject.CompareTag("Ice")
            || collision.gameObject.CompareTag("Rubber")
        )
        {
            // Only show dust if we fall from a certain height/speed
            if (collision.relativeVelocity.magnitude > impactThreshold)
            {
                if (landingVFXPrefab != null)
                {
                    ContactPoint contact = collision.contacts[0];
                    Vector3 spawnPos = contact.point + Vector3.up * 0.02f;
                    Instantiate(landingVFXPrefab, spawnPos, Quaternion.identity);
                }
            }
            // if (!collision.gameObject.CompareTag("Ground"))
            //  return;

            Vector3 gravityDir = gravityControl.GetGravityDirection();
            Vector3 up = -gravityDir;

            float verticalVelocity = Vector3.Dot(rb.linearVelocity, up);

            if (verticalVelocity > 0.1f)
                return;

            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, up) > 0.5f)
                {
                    isGrounded = true;
                    hasBurstCharge = true;
                    return;
                }
            }

            if (collision.gameObject.CompareTag("Ice"))
            {
                onIce = true;
                Debug.Log("ice");
            }
            if (collision.gameObject.CompareTag("Rubber"))
            {
                onRubber = true;
                Debug.Log("rubber");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            onIce = false;
        }
        if (collision.gameObject.CompareTag("Rubber"))
        {
            onRubber = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        HandleGroundCollision(collision);
        if (
            collision.gameObject.CompareTag("Ground")
            || collision.gameObject.CompareTag("Ice")
            || collision.gameObject.CompareTag("Rubber")
        )
        {
            // Avoid double jump if we're still moving upwards from a jump
            if (rb.linearVelocity.y <= 0.1f)
                if (collision.gameObject.CompareTag("Ground"))
                {
                    Vector3 normal = collision.contacts[0].normal;

                    // Surface has to face up enough to be floor for now
                    if (normal.y > 0.5f)
                    {
                        hasBurstCharge = true;
                        isGrounded = true;
                    }
                }
            if (collision.gameObject.CompareTag("Ice"))
            {
                Vector3 normal = collision.contacts[0].normal;

                onIce = true;
                // Surface has to face up enough to be floor for now
                foreach (ContactPoint contact in collision.contacts)
                    if (normal.y > 0.5f)
                    {
                        if (contact.normal.y > 0.5f)
                        {
                            isGrounded = true;
                            currentDashEnergy = maxDashEnergy;
                            hasBurstCharge = true;

                            break;
                        }
                    }
                Debug.Log("ice");
            }
            if (collision.gameObject.CompareTag("Rubber"))
            {
                Vector3 normal = collision.contacts[0].normal;

                onRubber = true;
                // Surface has to face up enough to be floor for now
                if (normal.y > 0.5f)
                {
                    hasBurstCharge = true;
                    isGrounded = true;
                }
                //burst used to  be here
                Debug.Log("rubber");
            }
        }
    }
}
