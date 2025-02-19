using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    public float damageAmount = 30f; // Damage the enemy takes per hit
    private float damageCooldown = 2f; // Cooldown time to prevent rapid hits
    private float currentCooldown = 0f; // Timer for cooldown

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
        // Check if the object hitting the enemy has the "PlayerWeapon" tag
        if (other.CompareTag("PlayerWeapon") && currentCooldown <= 0f)
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
            Debug.Log($"{gameObject.name} took {damageAmount} damage!");
            currentCooldown = damageCooldown; // Start cooldown
        }
    }
}
