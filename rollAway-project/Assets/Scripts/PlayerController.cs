using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    private Rigidbody rb;
    private Vector2 movementInput;
    public float speed;
    public float jumpPower = 7f;
    public float dashPower = 5f;
    public float bounceBoost = 14f;
    public TextMeshProUGUI countText;
    private int count;
    private float maxSpeed = 11.5f;
    public GameObject winTextObject;
    private bool hasBurstCharge = true;
    private InputActions inputActions;
    private bool isGrounded = true;
    [NonSerialized] public bool onIce = false;
    public float stickyMoveMultiplier = 4f;
    public float stickyClimbForce = 25f;
    private bool onRubber = false;
    private bool onSticky = false;
    private Vector3 stickyNormal;
    
    void Awake()
    {
        Instance = this;
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Dash.performed += OnDashPerformed;

        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Dash.performed -= OnDashPerformed;
        inputActions.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            if (onRubber)
            {
                rb.AddForce(Vector3.up * bounceBoost, ForceMode.Impulse);
            }
            else
            {

                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
            isGrounded = false;
        }
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (hasBurstCharge)
        {
            ExecuteBurst();
            hasBurstCharge = false;
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
        // change behavior of player movement if on ice (more slidey)
        if (onSticky)
        {
            rb.linearDamping = 2f;
            rb.angularDamping = 2f;
            // Wall climbing (no cielings)
            if (stickyNormal.y < 0.5f && stickyNormal.y > -0.5f)
            {
                Vector3 climbingMovement = Vector3.ProjectOnPlane(movement, stickyNormal);
                //keep ball attached and not fall off
                rb.AddForce(-stickyNormal*20f, ForceMode.Force);
                // offset gravity while climbing (not all the way though)
                rb.AddForce(Vector3.up *9.81f, ForceMode.Force);
                Vector3 climbDir = climbingMovement.normalized;
                // rb.AddForce(climbDir * stickyClimbForce, ForceMode.Acceleration);
                rb.AddForce(climbingMovement * (speed * stickyMoveMultiplier), ForceMode.Force);
                return;
            }
        }
        else if (onIce)
        {
            speed += 0.1f * Time.deltaTime;

            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.05f;
            // Apply movement
            rb.AddForce(movement * speed, ForceMode.Force);

            //clamp velocity
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
            }

            return;
        }
        else
        {
            speed = 10f;
            rb.linearDamping = 1f; // normal
            // rb.angularDamping = 1f;
        }
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
        Vector3 normal = collision.contacts[0].normal;
        if (collision.gameObject.CompareTag("Ground"))
        {

            // Surface has to face up enough to be floor for now
            if (normal.y > 0.5f)
            {
                hasBurstCharge = true;
                isGrounded = true;
            }
        }
        if (collision.gameObject.CompareTag("Ice"))
        {
            //Vector3 normal = collision.contacts[0].normal;
            
            onIce = true;
            // Surface has to face up enough to be floor for now
            if (normal.y > 0.5f)
            {
                hasBurstCharge = true;
                isGrounded = true;
            }
            // Debug.Log("ice");
        }
        if (collision.gameObject.CompareTag("Rubber"))
        {
            //Vector3 normal = collision.contacts[0].normal;
            
            onRubber = true;
            // Surface has to face up enough to be floor for now
            if (normal.y > 0.5f)
            {
                hasBurstCharge = true;
                isGrounded = true;
            }
            // Debug.Log("rubber");
        }

        if (collision.gameObject.CompareTag("Flypaper"))
        {
            //Vector3 normal = collision.contacts[0].normal;
            stickyNormal = normal;
            onSticky = true;
            if (normal.y > 0.5f)
            {
                hasBurstCharge = true;
                isGrounded = true;
            }
            // Debug.Log("Flypaper");
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

        if (collision.gameObject.CompareTag("Flypaper"))
        {
            onSticky = false;
            stickyNormal = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||  collision.gameObject.CompareTag("Ice") || collision.gameObject.CompareTag("Rubber") || collision.gameObject.CompareTag("Flypaper"))
        {
            // Avoid double jump if we're still moving upwards from a jump
            if (rb.linearVelocity.y <= 0.1f)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f)
                    {
                        isGrounded = true;
                        hasBurstCharge = true;
                        break;
                    }
                }
            }
        }
    }
}
