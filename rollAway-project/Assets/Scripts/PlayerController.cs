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
    public float stickyMoveMultiplier = 4f;
    public float stickyClimbForce = 25f;
    public GameObject cameraObject;
    private GravityControl gravityControl;
    [NonSerialized] public bool onIce = false;
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
        gravityControl = GetComponent<GravityControl>();

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
            if (!onRubber)
            {
                Vector3 gravityDir = gravityControl.GetGravityDirection();
                Vector3 up = -gravityDir;

                rb.AddForce(up * jumpPower, ForceMode.Impulse);
            }
            else
            {
                {
                    rb.AddForce(Vector3.up * bounceBoost, ForceMode.Impulse);
                }

                isGrounded = false;
            }
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
        {if (onSticky){
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
        Vector3 normal = collision.contacts[0].normal;
        if (collision.gameObject.CompareTag("Ground"))
        {
    }

    private void OnCollisionStay(Collision collision)
    {
        HandleGroundCollision(collision);
    }

    private void HandleGroundCollision(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
            return;

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
            // Debug.Log("ice");
        }
        if (collision.gameObject.CompareTag("Rubber"))
        {
            //Vector3 normal = collision.contacts[0].normal;
            
            onRubber = true;
            // Surface has to face up enough to be floor for now
            if (normal.y > 0.5f)
            {
                Vector3 normal = collision.contacts[0].normal;

                onIce = true;
                // Surface has to face up enough to be floor for now
                if (normal.y > 0.5f)
                {
                    hasBurstCharge = true;
                    isGrounded = true;
                }
                Debug.Log("ice");
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
                Debug.Log("rubber");
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
}
