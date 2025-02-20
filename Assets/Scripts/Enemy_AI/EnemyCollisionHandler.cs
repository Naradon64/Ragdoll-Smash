using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    public float damageAmount = 30f; // Damage the enemy takes per hit
    private float damageCooldown = 2f; // Cooldown time to prevent rapid hits
    private float currentCooldown = 0f; // Timer for cooldown
    private EnemyHealth enemyHealth; // Reference to EnemyHealth component

    private void Start()
    {
        // Get the EnemyHealth component at the start
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth component not found on " + gameObject.name);
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
        // Check if the object hitting the enemy has the "PlayerWeapon" tag
        if (other.CompareTag("PlayerWeapon") && currentCooldown <= 0f)
        {
            enemyHealth?.TakeDamage(damageAmount); // เรียกใช้ TakeDamage() ใน EnemyHealth.cs 
            Debug.Log($"{gameObject.name} took {damageAmount} damage!");
            currentCooldown = damageCooldown; // Start cooldown
        }
    }
}
