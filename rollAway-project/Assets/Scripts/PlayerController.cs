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
    private Vector2 movementInput;
    private int count;
    private bool isGrounded = true;
    private Vector3 lastPosition;
    private InputActions inputActions;

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
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
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
        // This resets the vertical velocity to zero, allowing for a consistent dash
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // The forward dash logic
        Vector3 dashDirection = new Vector3(movementInput.x, 0, movementInput.y);

        // If there's no input, we can default to the current facing direction or forward
        if (dashDirection == Vector3.zero)
            dashDirection = rb.linearVelocity.normalized;
        if (dashDirection == Vector3.zero)
            dashDirection = Vector3.forward;

        // Apply the forces for the dash
        rb.AddForce(dashDirection * dashPower, ForceMode.VelocityChange);

        // Adding slight upward lift
        rb.AddForce(Vector3.up * 3f, ForceMode.VelocityChange);

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
        Vector3 movement = new Vector3(movementInput.x, 0.0f, movementInput.y);
        rb.AddForce(movement * speed);
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy the current object
            Destroy(gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text =
                "HAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHA";
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            // Only show dust if we fall from a certain height/speed
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
            if (landingVFXPrefab != null)
            {
                ContactPoint contact = collision.contacts[0];
                
                // Lift the spawn point up slightly so the dust is fully visible
                Vector3 spawnPos = contact.point + Vector3.up * 0.02f;
                
                Instantiate(landingVFXPrefab, spawnPos, Quaternion.identity);
            }
        }
            Vector3 normal = collision.contacts[0].normal;

            // Surface has to face up enough to be floor for now
            if (normal.y > 0.5f)
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Avoid double jump if we're still moving upwards from a jump
            if (rb.linearVelocity.y <= 0.1f)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f)
                    {
                        isGrounded = true;
                        break;
                    }
                }
            }
        }
    }
}
