using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float drag = 4f;  // Drag when grounded
    public float airDrag = 0.1f;  // Drag when in the air (lower value)
    public float jumpForce = 10f;  // Force applied when jumping

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isGrounded;
    private bool isRagdoll;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        // Freeze rotation to prevent character from tipping over
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!isRagdoll)  // Only process input when not in ragdoll state
        {
            // Get player input
            MyInput();
            // Check if the player is on the ground
            CheckGroundedStatus();

            // Check for jump input (spacebar)
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isRagdoll)  // Only move player when not in ragdoll state
        {
            // Apply movement
            MovePlayer();
        }
    }

    private void MyInput()
    {
        // Get raw input for horizontal (A/D, Left/Right Arrow) and vertical (W/S, Up/Down Arrow) directions
        horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        verticalInput = Input.GetAxis("Vertical");     // W/S or Up/Down Arrow
    }

    private void MovePlayer()
    {
        // Get the camera's forward and right direction
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // Flatten the directions (ignore the Y-axis to avoid moving vertically)
        forward.y = 0f;
        right.y = 0f;

        // Normalize the directions so the player doesn't move faster diagonally
        forward.Normalize();
        right.Normalize();

        // Create a movement vector relative to the camera
        moveDirection = forward * verticalInput + right * horizontalInput;

        // Apply movement force to the Rigidbody
        rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);

        // Adjust drag based on whether the player is grounded or not
        if (isGrounded)
        {
            rb.linearDamping = drag;  // Apply full drag when on the ground
        }
        else
        {
            rb.linearDamping = airDrag;  // Apply reduced drag when in the air
        }
    }

    private void CheckGroundedStatus()
    {
        // Cast a ray downward to check if the player is on the ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            // If the ray hits something within 1 unit below the player, the player is grounded
            isGrounded = true;
        }
        else
        {
            // Otherwise, the player is in the air
            isGrounded = false;
        }
    }

    private void Jump()
    {
        // Apply an upward force to make the player jump
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Public method to toggle ragdoll state
    public void SetRagdollState(bool state)
    {
        isRagdoll = state;
        // You can implement additional logic to enable/disable Ragdoll components
    }
}
