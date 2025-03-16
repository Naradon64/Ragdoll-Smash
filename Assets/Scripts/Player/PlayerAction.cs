using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private GameObject heldItem;
    
    [SerializeField] private Transform rightArm; // Assign in Inspector
    [SerializeField] private Transform leftArm;  // Assign in Inspector

    private Vector3 prevRightArmPos;
    private Vector3 prevLeftArmPos;
    private float velocityThreshold = 5.0f; // Increased threshold to avoid accidental throws
    private float angleThreshold = 30.0f; // Threshold for arm angle (in degrees)
    private float cooldownTime = 0.5f; // Cooldown to prevent multiple throws in rapid succession
    private float lastThrowTime;

    private PlayerCollisionHandler playerCollisionHandler;

    private void Start()
    {
        playerCollisionHandler = GetComponent<PlayerCollisionHandler>();

        // Initialize previous positions
        if (rightArm != null) prevRightArmPos = rightArm.position;
        if (leftArm != null) prevLeftArmPos = leftArm.position;
        lastThrowTime = Time.time; // Initialize last throw time
    }

    private void Update()
    {
        // Calculate velocities
        if (rightArm != null && leftArm != null)
        {
            Vector3 rightArmVelocity = (rightArm.position - prevRightArmPos) / Time.deltaTime;
            Vector3 leftArmVelocity = (leftArm.position - prevLeftArmPos) / Time.deltaTime;

            // Store current position for the next frame
            prevRightArmPos = rightArm.position;
            prevLeftArmPos = leftArm.position;

            // Check if velocity exceeds the threshold and if the arm is swinging downward
            if (heldItem != null && heldItem.CompareTag("Throwable"))
            {
                // Check if enough time has passed since the last throw
                if (Time.time - lastThrowTime < cooldownTime) return;

                // Right arm throw detection
                if (rightArmVelocity.magnitude > velocityThreshold && rightArmVelocity.y < -0.4f)
                {
                    if (IsArmInThrowingPosition(rightArm))
                    {
                        Debug.Log($"Right arm swung down fast: {rightArmVelocity}, throwing item.");
                        playerCollisionHandler.ThrowItem(rightArmVelocity);
                        lastThrowTime = Time.time; // Update last throw time
                    }
                }
                // Left arm throw detection
                else if (leftArmVelocity.magnitude > velocityThreshold && leftArmVelocity.y < -0.4f)
                {
                    if (IsArmInThrowingPosition(leftArm))
                    {
                        Debug.Log($"Left arm swung down fast: {leftArmVelocity}, throwing item.");
                        playerCollisionHandler.ThrowItem(leftArmVelocity);
                        lastThrowTime = Time.time; // Update last throw time
                    }
                }
            }
        }
    }

    private bool IsArmInThrowingPosition(Transform arm)
    {
        // Check if the arm's current angle is within a throwing range
        Vector3 armDirection = arm.forward; // Forward direction of the arm
        float angle = Vector3.Angle(armDirection, Vector3.down); // Angle between arm's forward and the downward direction

        if (angle < angleThreshold)
        {
            return true; // Arm is pointing downwards, ready to throw
        }

        return false;
    }

    public void SetHeldItem(GameObject item)
    {
        heldItem = item;
    }
}
