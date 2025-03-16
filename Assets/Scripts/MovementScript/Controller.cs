using UnityEngine;

public class Controller : MonoBehaviour
{
    public Orientation orientation;  // Reference to the Orientation script

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Mouse X");  // Mouse X movement
        float verticalInput = Input.GetAxis("Mouse Y");    // Mouse Y movement

        orientation.Rotate(horizontalInput, verticalInput);  // Rotate based on input
    }
}
