using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private GameObject heldItem;
    private GameObject throwableBallPrefab;  // Reference to the throwable ball prefab
    private float respawnInterval = 2f;  // Time after which the ball respawns (in seconds)
    private float lastPickupTime;
    private bool ballPickedUp = false;  // Flag to track if the ball has been picked up

    [SerializeField] private Transform rightArm; // Assign in Inspector
    [SerializeField] private Transform leftArm;  // Assign in Inspector

    private Vector3 prevRightArmPos;
    private Vector3 prevLeftArmPos;
    private float velocityThreshold = 10.0f; // Increased threshold to avoid accidental throws
    private float cooldownTime = 0.5f; // Cooldown to prevent multiple throws in rapid succession
    private float lastThrowTime;

    private PlayerCollisionHandler playerCollisionHandler;

    [SerializeField] private Vector3 ballStartPosition; // The starting position of the ball

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

            // Debug: Log arm velocities
            // Debug.Log($"<size=30>Right Arm Velocity: {rightArmVelocity.magnitude}, Left Arm Velocity: {leftArmVelocity.magnitude}</size>");

            // Check if velocity exceeds the threshold and if the arm is swinging downward
            if (heldItem != null && heldItem.CompareTag("Throwable"))
            {
                // Check if enough time has passed since the last throw
                if (Time.time - lastThrowTime < cooldownTime)
                {
                    Debug.Log("Cooldown active, throw not allowed yet.");
                    return;
                }

                // Right arm throw detection
                if (rightArmVelocity.magnitude > velocityThreshold && rightArmVelocity.y < -0.4f)
                {
                    Debug.Log($"Right arm swung down fast: {rightArmVelocity}, throwing item.");
                    playerCollisionHandler.ThrowItem(rightArmVelocity);
                    lastThrowTime = Time.time; // Update last throw time
                }
                // Left arm throw detection
                else if (leftArmVelocity.magnitude > velocityThreshold && leftArmVelocity.y < -0.4f)
                {
                    Debug.Log($"Left arm swung down fast: {leftArmVelocity}, throwing item.");
                    playerCollisionHandler.ThrowItem(leftArmVelocity);
                    lastThrowTime = Time.time; // Update last throw time
                }
            }
        }

        // Check if 5 seconds have passed since the ball was picked up and regenerate the ball
        if (ballPickedUp && Time.time - lastPickupTime >= respawnInterval)
        {
            GenerateNewBall();
            ballPickedUp = false;  // Reset flag after regenerating the ball
        }
    }

    // When an item is picked up
    public void SetHeldItem(GameObject item)
    {
        heldItem = item;
        if (heldItem.CompareTag("Throwable"))
        {
            throwableBallPrefab = item; // Store reference to the throwable ball prefab
            item.SetActive(true); // Ensure the ball is active when picked up
            lastPickupTime = Time.time; // Initialize pickup time
            ballPickedUp = true;  // Set flag to true to track the ball has been picked up
        }
    }

    // Generate a new ball at the same position
    private void GenerateNewBall()
    {
        if (throwableBallPrefab != null)
        {
            // Instantiate a new ball at the starting position
            GameObject newBall = Instantiate(throwableBallPrefab, ballStartPosition, Quaternion.identity);

            // Reset the scale to ensure it doesn't shrink
            newBall.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
    }
}
