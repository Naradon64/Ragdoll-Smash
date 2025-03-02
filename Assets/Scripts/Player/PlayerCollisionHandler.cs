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

    private void Start()
    {
        // Get the PlayerHealth component from the player
        playerHealth = GetComponent<PlayerHealth>();
        playerAction = GetComponent<PlayerAction>();
        playerColliders = GetComponentsInChildren<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.tag == "Enemy" && currentDamageCooldown <= 0f)
        // {
        //     Debug.Log($"ชน {collision.gameObject.name}");
        //     if (playerHealth != null)
        //     {
        //         playerHealth.TakeDamage(10f);
        //     }
        //     // Reset damage cooldown
        //     currentDamageCooldown = damageCooldown;
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyWeapon" && currentDamageCooldown <= 0f)
        {
            Debug.Log($"โดน {other.gameObject.name} โจมตี");
            // Call the TakeDamage method from PlayerHealth
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10f);
            }
            // Reset damage cooldown
            currentDamageCooldown = damageCooldown;
        }

        // Check if the item is in range of either hand and if it's not already held
        if (((other.CompareTag("Item") || (other.CompareTag("PlayerWeapon"))) && heldItem == null && currentCooldown <= 0))
        {
            // Determine which hand is closer
            float distanceToRight = Vector3.Distance(other.transform.position, rightHand.position);
            float distanceToLeft = Vector3.Distance(other.transform.position, leftHand.position);

            Transform chosenHand = distanceToRight < distanceToLeft ? rightHand : leftHand;

            PickupItem(other.gameObject, chosenHand);
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

            // Unparent the item
            heldItem.transform.SetParent(null);

            // Re-enable physics
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            // Restore collision between the player and the item
            Collider[] itemColliders = heldItem.GetComponentsInChildren<Collider>();
            foreach (Collider itemCollider in itemColliders)
            {
                foreach (Collider playerCollider in playerColliders)
                {
                    Physics.IgnoreCollision(playerCollider, itemCollider, false);
                }
            }

            heldItem = null; // Clear the held item
            currentHand = null; // Reset the hand transform

            // Inform PlayerAction that no item is held anymore
            playerAction.SetHeldItem(null);
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
                rb.velocity = direction * initialSpeed;

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
        // หลังจากที่ set cooldown ใน PickupItem พอปล่อยก็จะนับเวลาจนกว่าจะหยิบได้อีก
        if (currentCooldown > 0 && heldItem == null)
        {
            currentCooldown -= Time.deltaTime;
            // Debug.Log($"Pick up cooldown: {currentCooldown}");
        }

        // Update the damage cooldown
        if (currentDamageCooldown > 0f)
        {
            currentDamageCooldown -= Time.deltaTime;
            // Debug.Log($"Damage taken cooldown: {currentDamageCooldown}");
        }

        if (heldItem != null)
        {
            if (currentHand == rightHand && Input.GetKeyDown(KeyCode.E))
            {
                DropItem();
            }
            else if (currentHand == leftHand && Input.GetKeyDown(KeyCode.Q))
            {
                DropItem();
            }
        }

        Transform targetEnemy = FindClosestEnemyInFront();
        Debug.Log($"The infront enemy is {targetEnemy}");
        lockedEnemyText.text = $"Locked Target: {targetEnemy}";
    }
}
