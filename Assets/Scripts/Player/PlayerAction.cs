using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("Charging Settings")]
    [SerializeField] private Transform chest; // Assign a chest or center body reference in Inspector
    [SerializeField] private Slider chargeSlider; // Assign UI Slider from canvas
    [SerializeField] private float maxChargeTime = 2f; // How long to hold arms up to fully charge
    private float chargeAmount = 0f;
    private bool isCharging = false;
    private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI chargingMessageText;  // Text for "Charging..."
    [SerializeField] private TextMeshProUGUI healedMessageText;    // Text for "Charged!" and "Healed" // Drag the UI text here in the Inspector
    private Coroutine messageCoroutine;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();

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

        HandleCharging();

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

    private void HandleCharging()
    {
        if (rightArm == null || leftArm == null || chest == null || chargeSlider == null) return;

        bool armsUp = rightArm.position.y > chest.position.y + 0.3f &&
                    leftArm.position.y > chest.position.y + 0.3f;

        // If the arms are up, continue charging
        if (armsUp)
        {
            isCharging = true;

            // If just started charging, display "Charging..."
            if (chargeAmount == 0f)
            {
                ShowChargingMessage("Charging...");
            }

            chargeAmount += Time.deltaTime;
            chargeAmount = Mathf.Clamp(chargeAmount, 0, maxChargeTime);
        }
        else
        {
            // If the pose is canceled, show "Charge canceled!" for a brief moment
            if (isCharging && chargeAmount < maxChargeTime)
            {
                ShowChargingMessage("Charge canceled!");
                // Reset the charging state after a brief delay
                StartCoroutine(HideChargedMessageWithDelay(1f));  // 1-second delay for "Charge canceled!"
            }

            isCharging = false;
            chargeAmount = 0;
        }

        chargeSlider.value = chargeAmount / maxChargeTime;

        // If charge is full, heal the player
        if (chargeAmount >= maxChargeTime)
        {
            Debug.Log("<color=green><size=20>Charged!</size></color>");

            float healAmount = 10f;

            // Heal only if not at max health
            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.Heal(healAmount);
                Debug.Log($"<color=cyan>Healed for {healAmount} HP!</color>");
                ShowHealedMessage($"Healed for {healAmount} HP!");
                // Reset the charging state after a brief delay
                StartCoroutine(HideHealedMessageWithDelay(1f));  // 1-second delay for "Charge canceled!"
            }
            else
            {
                ShowHealedMessage("Charged!");
                // Reset the charging state after a brief delay
                StartCoroutine(HideHealedMessageWithDelay(1f));  // 1-second delay for "Charge canceled!"
            }

            chargeAmount = 0; // Reset after full charge
        }
    }

    private void ShowChargingMessage(string message)
    {
        if (chargingMessageText == null) return;

        chargingMessageText.text = message;
        chargingMessageText.gameObject.SetActive(true);
    }

    private void ShowHealedMessage(string message)
    {
        if (healedMessageText == null) return;

        // Show healed message and hide the charging message
        healedMessageText.text = message;
        healedMessageText.gameObject.SetActive(true);
    }

    private IEnumerator HideChargedMessageWithDelay(float delayTime)
    {
        // Wait for the given time and then hide the charging message
        yield return new WaitForSeconds(delayTime);

        chargingMessageText.gameObject.SetActive(false); // Hide "Charging..." or "Charge canceled!" message after delay
    }
    private IEnumerator HideHealedMessageWithDelay(float delayTime)
    {
        // Wait for the given time and then hide the charging message
        yield return new WaitForSeconds(delayTime);

        healedMessageText.gameObject.SetActive(false); // Hide "Charging..." or "Charge canceled!" message after delay
    }
}
