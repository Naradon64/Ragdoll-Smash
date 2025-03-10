using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float airDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    
    Rigidbody rb;

    // [Header("Rotation")]
    // public float rotationSpeed = 10f; // Adjust rotation speed as needed

    private Animator animator;
    private float deadZone = 0.1f;
    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();

        SpeedControl();
        if (horizontalInput != 0 || verticalInput != 0)
        {
            animator.SetBool("isWalking", true);
            Debug.Log("works");
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // Adjust drag based on whether the player is grounded or not
        if (grounded)
        {
            rb.linearDamping = groundDrag;  // Apply full drag when on the ground
        }
        else
        {
            rb.linearDamping = airDrag;  // Apply reduced drag when in the air
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        // RotatePlayer(); // Add rotation in FixedUpdate
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        Transform modelSubParent = transform.Find("pico_chan_chr_pico_00");

        if (modelSubParent != null)
        {
            animator = modelSubParent.GetComponent<Animator>();
            Debug.Log("skibidi");
        }
        else
        {
            Debug.LogError("Model sub-parent 'pico_chan_chr_pico_00' not found!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found on 'pico_chan_chr_pico_00'!");
        }
    }

    private void MyInput()
    {
        horizontalInput = Mathf.Abs(Input.GetAxis("Horizontal")) < deadZone ? 0 : Input.GetAxis("Horizontal");
        verticalInput = Mathf.Abs(Input.GetAxis("Vertical")) < deadZone ? 0 : Input.GetAxis("Vertical");

        // Update Animator parameters
        animator.SetFloat("Horizontal", horizontalInput);
        animator.SetFloat("Vertical", verticalInput);

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    // private void RotatePlayer()
    // {
    //     if (moveDirection.magnitude >= 0.1f) // Check if there is movement
    //     {
    //         Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    //         rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    //     }
    // }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y celocity to always jump same height
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}