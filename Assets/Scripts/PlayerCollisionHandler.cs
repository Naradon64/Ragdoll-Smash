using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private GameObject heldItem; // The item currently held by the hand
    private Transform currentHand; // The transform of the hand that picked up the item
    private float pickupCooldown = 1f; // Cooldown time in seconds before picking up again
    private float currentCooldown = 0f; // Current cooldown timer
    // private bool canPickup = true; // A flag to prevent immediate pickup after dropping
    public Transform rightHand;
    public Transform leftHand;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            Debug.Log($"โดน {collision.gameObject.name} โจมตี");
        }

        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log($"ชน {collision.gameObject.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the item is in range of either hand and if it's not already held
        if (other.CompareTag("Item") && heldItem == null && currentCooldown <= 0)
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
            
            Collider itemCollider = item.GetComponent<Collider>();
            if (itemCollider != null)
            {
                // Set the collider to trigger when picking up
                itemCollider.isTrigger = true; 
            }
            rb.useGravity = false;
        }

        // Parent the item to the hand (so it moves with it)
        item.transform.SetParent(hand);
        item.transform.localPosition = Vector3.zero; 
        item.transform.localRotation = Quaternion.identity;

        heldItem = item; // Store the held item
        currentHand = hand; // Save which hand picked it up

        // Start cooldown after picking up the item
        currentCooldown = pickupCooldown;
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

                Collider itemCollider = heldItem.GetComponent<Collider>();
                if (itemCollider != null)
                {
                    itemCollider.isTrigger = false; // Restore normal collision
                }
                rb.useGravity = true;
            }

            heldItem = null; // Clear the held item
            currentHand = null; // Reset the hand transform
        }
    }

    private void Update()
    {
        // หลังจากที่ set cooldown ใน PickupItem พอปล่อยก็จะนับเวลาจนกว่าจะหยิบได้อีก
        if (currentCooldown > 0 && heldItem == null)
        {
            currentCooldown -= Time.deltaTime;
            Debug.Log(currentCooldown);
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
    }
}
