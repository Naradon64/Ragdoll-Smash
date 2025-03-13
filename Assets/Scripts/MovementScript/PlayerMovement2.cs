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
    private bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    private Animator animator;
    private float deadZone = 0.1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        Transform modelSubParent = transform.Find("pico_chan_chr_pico_00");
        if (modelSubParent != null)
        {
            animator = modelSubParent.GetComponent<Animator>();
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

    private void Update()
{

    // DEBUG: Draw the ground detection ray in the Scene view
    // Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.6f), Color.red);

    bool wasGrounded = grounded;
    grounded = IsGrounded(); // Use the new function

    if (!wasGrounded && grounded)  
    {
        print("Land");
        Land();
    }

    // If player is in the air and moving downward, mark as falling
    if (!grounded && rb.linearVelocity.y < -0.1f)
    {
        animator.SetBool("isFalling", true);
        animator.SetBool("isGrouded", false);
    }

    MyInput();
    SpeedControl();

    // Apply correct drag
    rb.linearDamping = grounded ? groundDrag : airDrag;
}


    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Mathf.Abs(Input.GetAxis("Horizontal")) < deadZone ? 0 : Input.GetAxis("Horizontal");
        verticalInput = Mathf.Abs(Input.GetAxis("Vertical")) < deadZone ? 0 : Input.GetAxis("Vertical");

        // Update Animator parameters
        animator.SetFloat("Horizontal", horizontalInput);
        animator.SetFloat("Vertical", verticalInput);

        // Jump logic
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Apply force
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Limit velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
{
    if (!grounded) return; // Prevent jumping in mid-air

    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset vertical velocity

    readyToJump = false;

    animator.SetBool("isJumping", true);
    animator.SetBool("isFalling", false);
    animator.SetBool("isGrouded", false);

    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    Invoke(nameof(ResetJump), jumpCooldown);
}


    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Land()
    {
        readyToJump = true;

        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
        animator.SetBool("isGrouded", true);

        // Slight delay before resetting animations
        Invoke(nameof(ResetLanding), 0.1f);  
    }

private void ResetLanding()
{
    animator.SetBool("isGrouded", false);
    animator.SetBool("isFalling", false);
}
private bool IsGrounded()
{
    float groundCheckDistance = 0.2f; // Small ray distance for accurate ground detection
    Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Start slightly above the feet

    // Cast a ray downward
    bool hit = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, whatIsGround);

    // Debug the ray in scene view
    Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, hit ? Color.green : Color.red);

    return hit;
}

}
