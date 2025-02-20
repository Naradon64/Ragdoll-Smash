using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private GameObject heldItem; // The item currently held by the player
    private Transform rightHand;
    private Transform leftHand;

    // Reference to PlayerCollisionHandler for item picking logic
    private PlayerCollisionHandler playerCollisionHandler;

    private void Start()
    {
        // Get references to the hands and the PlayerCollisionHandler component
        rightHand = transform.Find("RightHand");
        leftHand = transform.Find("LeftHand");
        playerCollisionHandler = GetComponent<PlayerCollisionHandler>(); // Assuming PlayerCollisionHandler is on the same object
    }

    private void OnTriggerEnter(Collider other)
    {
        // // Check if the player is holding an item with the tag "PlayerWeapon"
        // if (heldItem != null && heldItem.CompareTag("PlayerWeapon"))
        // {
        //     // Check if it triggers with an enemy
        //     if (other.CompareTag("Enemy"))
        //     {
        //         Debug.Log($"โจมตีใส่ {other.gameObject.name}");
        //     }
        // }
    }

    // To assign held item from PlayerCollisionHandler
    public void SetHeldItem(GameObject item)
    {
        heldItem = item;
    }
}
