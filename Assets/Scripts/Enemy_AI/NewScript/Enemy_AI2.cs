using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI2 : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 10f;  // Range to detect the player
    public float attackRange = 2f;    // Range to perform attacks
    public float attackCooldown = 1.5f; // Time between attacks
    public float rotationSpeed = 5f;    // Speed at which the enemy rotates to face the player

    [Header("Movement Settings")]
    public float movementSpeed = 2f; // Speed at which the enemy moves DEFAULT AT 3.5

    [Header("Animation Settings")]
    public float animationSpeedMultiplier = 0.6f; // Multiplier for animation speed 

    [Header("References")]
    public Transform player;           // Reference to the player
    private NavMeshAgent navMeshAgent; // NavMeshAgent for movement
    private Animator animator;         // Reference to the Animator on the model

    private float distanceToPlayer;    // Distance to the player
    private float lastAttackTime = 0;  // Tracks time since the last attack

    [Header("Weapon Settings")]
    public Collider swordCollider; // Reference to the sword's collider

    private Rigidbody rb;  // Rigidbody for applying force during knockback
    private bool isKnockedBack = false; // Flag to check if knockback is active
    private Vector3 knockbackDirection; // Direction for the knockback force
    

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (!navMeshAgent)
            Debug.LogError("NavMeshAgent not found on Enemy!");
        if (!animator)
            Debug.LogError("Animator not found on the Model!");

        // Set the initial movement speed of the NavMeshAgent
        navMeshAgent.speed = movementSpeed;

        // Set the initial animation speed
        animator.speed = animationSpeedMultiplier;

        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
    }

    void Update()
    {
        // If the enemy is dead, stop all actions
        if (animator.GetBool("isDead"))
        {
            // Stop all actions if the enemy is dead
            StopAllActions();
            return;
        }

        if (isKnockedBack)
        {
            // Apply knockback if the enemy is in the knocked-back state
            ApplyKnockback(knockbackDirection);  // Applying the knockback force
        }
        else
        {
            // Regular AI behavior
            HandleAIState();
        }
    }


    void HandleAIState()
    {
        // Normal AI behavior (Chase, Idle, Attack)
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            HandleAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            HandleChase();
        }
        else
        {
            HandleIdle();
        }
    }
    void StopAllActions()
    {
        // Ensure NavMeshAgent is only stopped if it's on a valid NavMesh
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }
        animator.SetBool("isWalking", false);
        animator.ResetTrigger("AttackType1");
        animator.ResetTrigger("AttackType2");
    }
    void HandleChase()
{
    // Ensure NavMeshAgent is valid before setting destination
    if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.enabled)
    {
        // Move towards the player
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = player.position;

        // Set walking animation
        animator.SetBool("isWalking", true);

        // Reset attack trigger to ensure smooth transitions
        animator.ResetTrigger("AttackType1");
        animator.ResetTrigger("AttackType2");
    }
    else
    {
        Debug.LogWarning("NavMeshAgent is not valid during chase, cannot set destination.");
    }
}

    void HandleIdle()
    {
        // Stop moving only if the NavMeshAgent is valid and on a NavMesh
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }

        // Set idle animation
        animator.SetBool("isWalking", false);

        // Reset attack trigger
        animator.ResetTrigger("AttackType1");
        animator.ResetTrigger("AttackType2");
    }

    void HandleAttack()
    {
        // Stop movement during attack only if NavMeshAgent is valid and on a NavMesh
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }
        animator.SetBool("isWalking", false);

        // Make the enemy face the player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // Check if enough time has passed since the last attack
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("AttackType1");
            lastAttackTime = Time.time;
        }
    }

    public void EnableSwordCollider()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }
    }

    public void DisableSwordCollider()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
    }

    public void ApplyKnockback(Vector3 force, float duration = 2f)
{
    if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh) 
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;  
    }

    // Temporarily disable trigger so the collider interacts with the environment
    Collider enemyCollider = GetComponent<Collider>();
    if (enemyCollider != null)
    {
        enemyCollider.isTrigger = false;  // Disable the trigger so it will collide with walls
    }

    rb.isKinematic = false;
    rb.useGravity = true;
    rb.AddForce(force, ForceMode.Impulse);

    Invoke(nameof(RestoreMovement), duration);
}

private void RestoreMovement()
{
    rb.linearVelocity = Vector3.zero;
    rb.isKinematic = true;
    rb.useGravity = false;

    // Re-enable the trigger for the collider
    Collider enemyCollider = GetComponent<Collider>();
    if (enemyCollider != null)
    {
        enemyCollider.isTrigger = true;  // Enable trigger again to avoid collisions with the model
    }

    // Re-enable NavMeshAgent with proper position
    if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
    {
        transform.position = hit.position;  // Snap back to valid position
        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
    }
    else
    {
        Debug.LogWarning($"{gameObject.name} is not on a valid NavMesh! Cannot re-enable NavMeshAgent.");
    }
}

}
