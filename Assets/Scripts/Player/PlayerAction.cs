using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private GameObject heldItem;
    
    [SerializeField] private Transform rightArm; // Assign in Inspector
    [SerializeField] private Transform leftArm;  // Assign in Inspector

    private Vector3 prevRightArmPos;
    private Vector3 prevLeftArmPos;
    private float velocityThreshold = 10.0f; // Adjust based on testing

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

            // Check if velocity exceeds the threshold and drop the item
            if (heldItem != null)
            {
                if (rightArmVelocity.magnitude > velocityThreshold)
                {
                    Debug.Log($"Right arm moved too fast: {rightArmVelocity.magnitude}, dropping item.");
                    playerCollisionHandler.ForceDropItem();
                }
                else if (leftArmVelocity.magnitude > velocityThreshold)
                {
                    Debug.Log($"Left arm moved too fast: {leftArmVelocity.magnitude}, dropping item.");
                    playerCollisionHandler.ForceDropItem();
                }
            }
        }
    }

    public void SetHeldItem(GameObject item)
    {
        heldItem = item;
    }
}
