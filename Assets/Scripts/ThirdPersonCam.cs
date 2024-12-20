using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform Orientation;     // Camera's orientation (usually a child of the camera)
    public Transform Player;          // The Player (the body that moves)
    public Transform PlayerObj;       // The player object (usually a child of the player for rotating the character)
    public Rigidbody rb;              // Rigidbody of the player (for physics-based movement)

    public float rotationSpeed = 10f;
    public float moveSpeed = 5f;      // Adjust movement speed

    private float horizontalInput;
    private float verticalInput;
    private Vector3 inputDir;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get input for movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Get the direction relative to the camera's orientation
        inputDir = Orientation.forward * verticalInput + Orientation.right * horizontalInput;

        // Rotate the player object based on input and camera direction
        if (inputDir != Vector3.zero)
        {
            PlayerObj.forward = Vector3.Slerp(PlayerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }

    void FixedUpdate()
    {
        // Move the player based on input and camera direction
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Move the player based on the calculated direction
        if (inputDir != Vector3.zero)
        {
            rb.MovePosition(rb.position + inputDir.normalized * moveSpeed * Time.deltaTime);
        }
    }
}
