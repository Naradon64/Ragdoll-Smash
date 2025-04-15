using UnityEngine;
using System.Collections;
using UnityEngine.UI;  // For UI.Text or TextMeshProUGUI if using TMP
using TMPro;  // For TextMeshPro

public class PlayerCollisionHandler : MonoBehaviour
{
    private GameObject heldItem; // The item currently held by the hand
    private Transform currentHand; // The transform of the hand that picked up the item
    private float pickupCooldown = 1f; // Cooldown time in seconds before picking up again
    private float currentCooldown = 0f; // Current cooldown timer
    private float damageCooldown = 1f; // Cooldown time for taking damage
    private float currentDamageCooldown = 0f; // Current cooldown timer for taking damage
    public Transform rightHand;
    public Transform leftHand;
    private PlayerHealth playerHealth; // Reference to PlayerHealth script
    private PlayerAction playerAction; // Reference to PlayerAction to pass held item
    private Collider[] playerColliders;
    public TextMeshProUGUI lockedEnemyText;

    private Collider boxCollider; // Store the collider of the box when both hands are inside
    private bool rightHandInsideBox = false;
    private bool leftHandInsideBox = false;
    private bool boxPickedUp = false;
    private Vector3 boxOffset; // Store the relative offset between the hands and hold point
    private FixedJoint rightHandJoint;
    private FixedJoint leftHandJoint;
    public Transform modelOrientation;
    public GameObject playerRoot; // Assign the Player GameObject in the Inspector
    private WinMenu winMenu; // Reference to WinMenu

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerAction = GetComponent<PlayerAction>();

        playerColliders = playerRoot.GetComponentsInChildren<Collider>();

        Debug.Log($"Player colliders found: {playerColliders.Length}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyWeapon" && currentDamageCooldown <= 0f)
        {
            Debug.Log($"โดน {other.gameObject.name} โจมตี");
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10f);
            }
            currentDamageCooldown = damageCooldown;
        }

        if (other.CompareTag("Item") || other.CompareTag("PlayerWeapon") || other.CompareTag("Throwable"))
        {
            if (heldItem == null && currentCooldown <= 0)
            {
                float distanceToRight = Vector3.Distance(other.transform.position, rightHand.position);
                float distanceToLeft = Vector3.Distance(other.transform.position, leftHand.position);
                Transform chosenHand = distanceToRight < distanceToLeft ? rightHand : leftHand;
                PickupItem(other.gameObject, chosenHand);
            }
        }

        if (other.CompareTag("Movable"))
        {
            if (heldItem == null && currentCooldown <= 0) // ADDED COOLDOWN CHECK HERE
            {
                Debug.Log($"OnTriggerEnter: Movable object {other.gameObject.name} entered.");

                if (IsHandTouching(rightHand, other))
                {
                    Debug.Log("OnTriggerEnter: Right hand touching.");
                    rightHandInsideBox = true;
                    boxCollider = other;
                    Debug.Log($"OnTriggerEnter: rightHandInsideBox = {rightHandInsideBox}, leftHandInsideBox = {leftHandInsideBox}");
                    if (leftHandInsideBox)
                    {
                        Debug.Log("OnTriggerEnter: Both hands inside, grabbing box.");
                        GrabBox(other.gameObject);
                    }
                }
                if (IsHandTouching(leftHand, other))
                {
                    Debug.Log("OnTriggerEnter: Left hand touching.");
                    leftHandInsideBox = true;
                    boxCollider = other;
                    Debug.Log($"OnTriggerEnter: rightHandInsideBox = {rightHandInsideBox}, leftHandInsideBox = {leftHandInsideBox}");
                    if (rightHandInsideBox)
                    {
                        Debug.Log("OnTriggerEnter: Both hands inside, grabbing box.");
                        GrabBox(other.gameObject);
                    }
                }
            }
        }

        // Check if the player collides with the "WinFlag" to trigger the win menu
        if (other.CompareTag("WinFlag"))
        {
            Debug.Log("WinFlag triggered! Displaying win menu...");
            DisplayWinMenu();  // Call the function to display the win menu
        }
    }

    // Function to display the win menu
    private void DisplayWinMenu()
    {
        winMenu = FindFirstObjectByType<WinMenu>();
        if (winMenu != null)
        {
            winMenu.Win();
        }
        else
        {
            Debug.LogError("WinMenu UI element not found!");
        }
    }

    private bool IsHandTouching(Transform hand, Collider other)
    {
        Collider[] handColliders = hand.GetComponentsInChildren<Collider>();
        foreach (Collider handCollider in handColliders)
        {
            if (handCollider.isTrigger && handCollider.bounds.Intersects(other.bounds))
            {
                return true;
            }
        }
        return false;
    }

    private void GrabBox(GameObject box)
    {
        if (boxPickedUp) return;

        Rigidbody rb = box.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = box.AddComponent<Rigidbody>();
        }

        // Reduce mass and add damping to prevent strong collisions
        rb.mass = 0.5f;
        rb.linearDamping = 5f;
        rb.angularDamping = 5f;
        rb.isKinematic = false;
        rb.useGravity = false;

        if (rightHandJoint == null)
        {
            rightHandJoint = rightHand.gameObject.AddComponent<FixedJoint>();
            rightHandJoint.connectedBody = rb;
        }

        if (leftHandJoint == null)
        {
            leftHandJoint = leftHand.gameObject.AddComponent<FixedJoint>();
            leftHandJoint.connectedBody = rb;
        }

        heldItem = box;
        playerAction.SetHeldItem(heldItem);
        boxPickedUp = true;
        currentCooldown = pickupCooldown;

        boxCollider = box.GetComponent<Collider>();

        // Ignore collision between the player and the item
        Collider[] itemColliders = boxCollider.GetComponentsInChildren<Collider>();
        foreach (Collider itemCollider in itemColliders)
        {
            foreach (Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(playerCollider, itemCollider, true);
            }
        }
    }

    private void ReleaseBox()
    {
        Debug.Log("ReleaseBox: Releasing box.");

        if (rightHandJoint != null) Destroy(rightHandJoint);
        if (leftHandJoint != null) Destroy(leftHandJoint);
        rightHandJoint = null;
        leftHandJoint = null;

        Rigidbody rightHandRb = rightHand.GetComponent<Rigidbody>();
        if (rightHandRb != null)
        {
            Destroy(rightHandRb);
        }

        Rigidbody leftHandRb = leftHand.GetComponent<Rigidbody>();
        if (leftHandRb != null)
        {
            Destroy(leftHandRb);
        }

        rightHandInsideBox = false;
        leftHandInsideBox = false;

        if (boxCollider != null)
        {
            Rigidbody rb = boxCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Zero out movement to prevent flying
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // Reset physics
                rb.mass = 4f;
                rb.linearDamping = 0f;
                rb.angularDamping = 0.05f;
                rb.isKinematic = false;
                rb.useGravity = true;

                // Freeze temporarily for stability
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                StartCoroutine(UnfreezeRotation(rb, 1f));
            }

            // Re-enable collision with player after short delay
            StartCoroutine(ReenableCollisionWithDelay(boxCollider, 1.0f));
        }

        boxPickedUp = false;
        heldItem = null;
        boxCollider = null;

        playerAction.SetHeldItem(null);
    }

    private IEnumerator ReenableCollisionWithDelay(Collider boxCollider, float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider[] itemColliders = boxCollider.GetComponentsInChildren<Collider>();
        foreach (Collider itemCollider in itemColliders)
        {
            foreach (Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(playerCollider, itemCollider, false);
            }
        }

        Debug.Log("Re-enabled collision after delay.");
    }

    private IEnumerator UnfreezeRotation(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.constraints = RigidbodyConstraints.None;
    }

    private void PickupItem(GameObject item, Transform hand)
    {
        Debug.Log($"{hand.name} picked up {item.name}");
        Debug.Log($"PickupItem: currentCooldown = {currentCooldown}, heldItem = {heldItem}"); // ADDED DEBUG

        // Disable physics so the item doesn’t fall
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Ignore collision between the player and the item
        Collider[] itemColliders = item.GetComponentsInChildren<Collider>();
        foreach (Collider itemCollider in itemColliders)
        {
            foreach (Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(playerCollider, itemCollider, true);
            }
        }

        // Parent the item to the hand (so it moves with it)
        item.transform.SetParent(hand);
        item.transform.localPosition = Vector3.zero;

        // If the item is tagged as "PlayerWeapon", apply a local rotation fix
        if (item.CompareTag("PlayerWeapon"))
        {
            if (hand == leftHand)
            {
                item.transform.localRotation = Quaternion.Euler(180f, 0f, 0f); // Adjust the rotation here if needed
            }
            else
            {
                item.transform.localRotation = Quaternion.identity; // Reset rotation for the right hand
            }
        }
        else
        {
            // If it's not a "PlayerWeapon", leave the rotation unchanged
            item.transform.localRotation = Quaternion.identity;
        }

        heldItem = item; // Store the held item
        currentHand = hand; // Save which hand picked it up

        // Start cooldown after picking up the item
        currentCooldown = pickupCooldown;

        // Inform PlayerAction of the held item
        playerAction.SetHeldItem(item);
    }

    private void DropItem()
    {
        if (heldItem != null)
        {
            Debug.Log($"Dropped {heldItem.name}");

            heldItem.transform.SetParent(null);

            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            Collider[] itemColliders = heldItem.GetComponentsInChildren<Collider>();
            foreach (Collider itemCollider in itemColliders)
            {
                foreach (Collider playerCollider in playerColliders)
                {
                    Physics.IgnoreCollision(playerCollider, itemCollider, false);
                }
            }

            heldItem = null;
            currentHand = null;
            playerAction.SetHeldItem(null);
            boxPickedUp = false; // Reset the flag
        }
    }

    public void ThrowItem(Vector3 throwForce)
    {
        if (heldItem != null)
        {
            Debug.Log($"Threw {heldItem.name}");

            heldItem.transform.SetParent(null);

            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            Transform targetEnemy = FindClosestEnemyInFront();
            if (targetEnemy != null)
            {
                Vector3 targetPosition = targetEnemy.position;
                Vector3 startPosition = heldItem.transform.position;
                // Force the item to move directly toward the enemy, no physics nonsense
                Vector3 direction = (targetPosition - startPosition).normalized;
                float speed = 50f; // You can tune this to control how fast the ball hits the enemy

                rb.linearVelocity = direction * speed;

                // Optional: freeze rotation to make it fly straight
                rb.freezeRotation = true;

                // Optional: disable gravity so it flies perfectly
                rb.useGravity = false;

                // Apply gravity (Rigidbody already has useGravity = true)
            }
            else
            {
                // If no enemy, just throw forward (as before)
                rb.AddForce(transform.forward * throwForce.magnitude * 2.0f, ForceMode.Impulse);
            }

            Collider[] itemColliders = heldItem.GetComponentsInChildren<Collider>();
            foreach (Collider itemCollider in itemColliders)
            {
                foreach (Collider playerCollider in playerColliders)
                {
                    Physics.IgnoreCollision(playerCollider, itemCollider, false);
                }
                // Start the coroutine to restore collisions after a delay
                StartCoroutine(RestoreCollision(playerColliders, itemCollider, 0.5f)); // Adjust delay as needed
            }

            heldItem = null;
            currentHand = null;
            playerAction.SetHeldItem(null);
        }
    }

    private IEnumerator RestoreCollision(Collider[] playerColliders, Collider itemCollider, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Collider playerCollider in playerColliders)
        {
            Physics.IgnoreCollision(playerCollider, itemCollider, false);
        }

        Debug.Log("Restored collision between player and thrown item.");
    }

    private Transform FindClosestEnemyInFront()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        float maxAngle = 45f; // Max angle to consider (only in front of the player)
        float maxDistance = 40f; // Max distance to consider (adjust as needed)

        foreach (GameObject enemy in enemies)
        {
            Vector3 directionToEnemy = (enemy.transform.position - modelOrientation.position).normalized;
            float angle = Vector3.Angle(modelOrientation.forward, directionToEnemy); // Angle between forward and enemy

            if (angle < maxAngle) // Only consider enemies within 45 degrees in front
            {
                float distance = Vector3.Distance(modelOrientation.position, enemy.transform.position);
                if (distance < closestDistance && distance <= maxDistance) // Check against maxDistance
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }

        return closestEnemy;
    }

    private void Update()
    {
        if (currentCooldown > 0 && heldItem == null)
        {
            currentCooldown -= Time.deltaTime;
        }

        if (currentDamageCooldown > 0f)
        {
            currentDamageCooldown -= Time.deltaTime;
        }

        if (heldItem != null)
        {
            if (heldItem != null && heldItem.CompareTag("Movable"))
            {
                if (Input.GetButton("Fire1") || Input.GetButton("Fire2"))
                {
                    ReleaseBox();
                }
            }
            else if (currentHand == rightHand && Input.GetButton("Fire1"))
            {
                DropItem();
            }
            else if (currentHand == leftHand && Input.GetButton("Fire2"))
            {
                DropItem();
            }
        }

        // if (heldItem == null)
        // {
        //     Debug.Log($"Update: heldItem is null, currentCooldown = {currentCooldown}"); // ADDED DEBUG
        // }

        Transform targetEnemy = FindClosestEnemyInFront();
        // Debug.Log($"The infront enemy is {targetEnemy}");
        lockedEnemyText.text = $"Locked Target: {targetEnemy}";
    }
}
