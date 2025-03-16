using UnityEngine;

public class Orientation : MonoBehaviour
{
    public float rotationSpeed = 10f;  // Speed at which the player rotates

    // Method to handle player rotation based on input (such as mouse or joystick)
    public void Rotate(float horizontalInput, float verticalInput)
    {
        // Calculate horizontal and vertical rotation values
        float yaw = horizontalInput * rotationSpeed * Time.deltaTime;  // Rotation on the Y-axis
        float pitch = verticalInput * rotationSpeed * Time.deltaTime;  // Rotation on the X-axis (optional for vertical camera movement)

        // Apply the rotation
        transform.Rotate(Vector3.up * yaw);  // Rotate around the Y-axis (yaw)
        transform.Rotate(Vector3.right * pitch);  // Rotate around the X-axis (pitch) if needed (for camera up/down)
    }
}
