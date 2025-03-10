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

    private void Start()
    {
        // Get the PlayerHealth component from the player
        playerHealth = GetComponent<PlayerHealth>();
        playerAction = GetComponent<PlayerAction>();
        playerColliders = GetComponentsInChildren<Collider>();
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
        else if (other.CompareTag("Movable") && !boxPickedUp) // Only check if not already picked up
        {
            if (IsHandTouching(rightHand, other))
            {
                rightHandInsideBox = true;
                boxCollider = other;
            }
            if (IsHandTouching(leftHand, other))
            {
                leftHandInsideBox = true;
                boxCollider = other;
            }

            if (rightHandInsideBox && leftHandInsideBox)
            {
                PickupBox(boxCollider.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Movable") && !boxPickedUp) // Only check if not already picked up
        {
            if (!IsHandTouching(rightHand, other))
            {
                rightHandInsideBox = false;
                if (leftHandInsideBox)
                {
                    boxCollider = other;
                }
                else
                {
                    boxCollider = null;
                }
            }
            if (!IsHandTouching(leftHand, other))
            {
                leftHandInsideBox = false;
                if (rightHandInsideBox)
                {
                    boxCollider = other;
                }
                else
                {
                    boxCollider = null;
                }
            }

            if (boxCollider != null && boxCollider.gameObject == other.gameObject)
            {
                boxCollider = null;
            }
        }
    }

    private bool IsHandTouching(Transform hand, Collider other)
    {
        Collider[] handColliders = hand.GetComponentsInChildren<Collider>();
        foreach (Collider handCollider in handColliders)
        {
            if (handCollider.bounds.Intersects(other.bounds))
            {
                return true;
            }
        }
        return false;
    }

    private void PickupBox(GameObject box)
    {
        if (heldItem != null) return;

        Debug.Log($"Picked up box {box.name} with both hands.");

        Rigidbody rb = box.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] itemColliders = box.GetComponentsInChildren<Collider>();
        foreach (Collider itemCollider in itemColliders)
        {
            foreach (Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(playerCollider, itemCollider, true);
            }
        }

        // Calculate the average position of the two hands
        Vector3 averageHandPosition = (rightHand.position + leftHand.position) / 2f;
        Quaternion averageHandRotation = Quaternion.LookRotation(rightHand.position - leftHand.position);

        // Calculate the bounding box size of the object
        Bounds bounds = new Bounds(box.transform.position, Vector3.zero);
        foreach (Collider collider in itemColliders)
        {
            bounds.Encapsulate(collider.bounds);
        }

        // Calculate a dynamic offset based on the object's size
        boxOffset = bounds.size / 2f; // Adjust the divisor for desired offset
        boxOffset.z = Mathf.Abs(boxOffset.z);

        // Parent the box to the player and apply the offset
        box.transform.SetParent(transform);

        heldItem = box;
        currentHand = null;
        currentCooldown = pickupCooldown;
        playerAction.SetHeldItem(box);

        rightHandInsideBox = false;
        leftHandInsideBox = false;
        boxCollider = null;
        boxPickedUp = true;

        UpdateBoxPosition(); // Initial position update
    }

    private void UpdateBoxPosition()
{
    if (heldItem != null && heldItem.CompareTag("Movable"))
    {
        Vector3 averageHandPosition = (rightHand.position + leftHand.position) / 2f;
        Quaternion averageHandRotation = Quaternion.LookRotation(rightHand.position - leftHand.position);

        // Position the box in front of the hands, using the box's local forward direction
        heldItem.transform.position = averageHandPosition + heldItem.transform.forward * boxOffset.z;
        heldItem.transform.rotation = averageHandRotation;
    }
}

    private void PickupItem(GameObject item, Transform hand)
    {
        Debug.Log($"{hand.name} picked up {item.name}");

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
        item.transform.localRotation = Quaternion.identity;

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

    public void ForceDropItem()
    {
        DropItem();
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

                // Calculate direction and initial velocity
                Vector3 direction = (targetPosition - startPosition).normalized;
                float initialSpeed = throwForce.magnitude * 10.0f; // Adjust multiplier for speed

                // Set initial velocity
                rb.linearVelocity = direction * initialSpeed;

                // Apply gravity (Rigidbody already has useGravity = true)
            }
            else
            {
                // If no enemy, just throw forward (as before)
                rb.AddForce(transform.forward * throwForce.magnitude * 10.0f, ForceMode.Impulse);
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

        foreach (GameObject enemy in enemies)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToEnemy); // Angle between forward and enemy

            if (angle < maxAngle) // Only consider enemies within 45 degrees in front
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
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
            if (heldItem.CompareTag("Movable"))
            {
                if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
                {
                    DropItem();
                }
            }
            else if (currentHand == rightHand && Input.GetKeyDown(KeyCode.E))
            {
                DropItem();
            }
            else if (currentHand == leftHand && Input.GetKeyDown(KeyCode.Q))
            {
                DropItem();
            }
        }

        UpdateBoxPosition(); // Update the box position every frame

        Transform targetEnemy = FindClosestEnemyInFront();
        // Debug.Log($"The infront enemy is {targetEnemy}");
        lockedEnemyText.text = $"Locked Target: {targetEnemy}";
    }
}
