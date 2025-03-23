using UnityEngine;

public class EnemyCollisionHandler2 : MonoBehaviour
{
    public float damageAmount = 20f; // Damage the enemy takes per hit
    private float damageCooldown = 2f; // Cooldown time to prevent rapid hits
    private float currentCooldown = 0f; // Timer for cooldown
    private EnemyHealth2 enemyHealth; // Reference to EnemyHealth2 component

    private void Start()
    {
        // Get the EnemyHealth2 component at the start
        enemyHealth = GetComponent<EnemyHealth2>();
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth2 component not found on " + gameObject.name);
        }
    }

    private void Update()
    {
        // Reduce cooldown timer
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hitting the enemy has the "PlayerWeapon" tag and cooldown is finished
        if (other.CompareTag("PlayerWeapon") && currentCooldown <= 0f)
        {
            ApplyDamage(other.transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding with the enemy has the "Throwable" or "PlayerWeapon" tag and cooldown is finished
        if (collision.gameObject.CompareTag("Throwable") && currentCooldown <= 0f)
        {
            ApplyDamage(collision.transform);
        }
    }

    private void ApplyDamage(Transform damageSource)
    {
        // Calculate the damage direction (from the enemy to the player/throwable)
        Vector3 damageDirection = transform.position - damageSource.position;

        // Call TakeDamage with the damage and direction
        enemyHealth?.TakeDamage(damageAmount, damageDirection);

        // Log the damage
        Debug.Log($"{gameObject.name} took {damageAmount} damage from {damageSource.gameObject.tag}!");

        // Reset cooldown timer
        currentCooldown = damageCooldown;
    }
}
