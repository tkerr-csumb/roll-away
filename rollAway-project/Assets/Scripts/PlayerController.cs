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
    public TextMeshProUGUI countText;
    private int count;
    public GameObject winTextObject;
    private bool hasBurstCharge = true;
    private InputActions inputActions;
    private bool isGrounded = true;
    public GameObject cameraObject;
    private GravityControl gravityControl;

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
    void Start() {
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

    void OnJumpPerformed(InputAction.CallbackContext context) {
        if (isGrounded) {
            Vector3 gravityDir = gravityControl.GetGravityDirection();
            Vector3 up = -gravityDir;

            rb.AddForce(up * jumpPower, ForceMode.Impulse);
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

    void ExecuteBurst() {
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

    void FixedUpdate() {
        Vector3 gravityDir = gravityControl.GetGravityDirection();
        Vector3 up = -gravityDir;

        Vector3 camForward = cameraObject.transform.forward;
        Vector3 camRight = cameraObject.transform.right;

        camForward = Vector3.ProjectOnPlane(camForward, up).normalized;
        camRight = Vector3.ProjectOnPlane(camRight, up).normalized;

        Vector3 move = camForward * movementInput.y + camRight * movementInput.x;

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
            Vector3 normal = collision.contacts[0].normal;

            // Surface has to face up enough to be floor for now
            if (normal.y > 0.5f)
            {
                hasBurstCharge = true;
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
                        hasBurstCharge = true;
                        break;
                    }
                }
            }
        }
    }
}
