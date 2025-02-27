using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private GameObject heldItem;
    
    [SerializeField] private Transform rightArm; // Assign in Inspector
    [SerializeField] private Transform leftArm;  // Assign in Inspector

    private Vector3 prevRightArmPos;
    private Vector3 prevLeftArmPos;
    private float velocityThreshold = 5.0f; // Adjust based on testing

    private PlayerCollisionHandler playerCollisionHandler;

    private void Start()
    {
        playerCollisionHandler = GetComponent<PlayerCollisionHandler>();

        // Initialize previous positions
        if (rightArm != null) prevRightArmPos = rightArm.position;
        if (leftArm != null) prevLeftArmPos = leftArm.position;
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
            if (heldItem != null)
            {
                if (rightArmVelocity.magnitude > velocityThreshold && rightArmVelocity.y < -0.5f)
                {
                    Debug.Log($"Right arm swung down fast: {rightArmVelocity}, throwing item.");
                    playerCollisionHandler.ForceThrowItem(rightArmVelocity);
                }
                else if (leftArmVelocity.magnitude > velocityThreshold && leftArmVelocity.y < -0.5f)
                {
                    Debug.Log($"Left arm swung down fast: {leftArmVelocity}, throwing item.");
                    playerCollisionHandler.ForceThrowItem(leftArmVelocity);
                }
            }
        }
    }

    public void SetHeldItem(GameObject item)
    {
        heldItem = item;
    }
}
