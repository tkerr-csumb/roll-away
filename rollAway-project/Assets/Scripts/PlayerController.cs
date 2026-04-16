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

    void OnMove(InputAction.CallbackContext context)
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
        if (dashDirection == Vector3.zero) dashDirection = rb.linearVelocity.normalized;
        if (dashDirection == Vector3.zero) dashDirection = Vector3.forward;

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