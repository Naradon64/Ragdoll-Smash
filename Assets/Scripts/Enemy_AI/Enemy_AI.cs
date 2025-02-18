using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
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

    void Start()
    {
        // Get required components
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (!navMeshAgent)
            Debug.LogError("NavMeshAgent not found on Enemy!");
        if (!animator)
            Debug.LogError("Animator not found on the Model!");

        // Set the initial movement speed of the NavMeshAgent
        navMeshAgent.speed = movementSpeed;

        // Set the initial animation speed
        animator.speed = animationSpeedMultiplier;
    }

    void Update()
{
    // If the enemy is dead, stop all actions
        if (animator.GetBool("isDead"))
    {
        // Stop all animations immediately
        // animator.speed = 0f;  // This effectively freezes all animations
        navMeshAgent.isStopped = true; // Stop the nav mesh agent (no movement)

        // Ensure no other triggers are activated
        animator.SetBool("isWalking", false);
        animator.ResetTrigger("AttackType1");
        animator.ResetTrigger("AttackType2");
        return; // Exit the update to stop further logic
    }

    // Continue normal AI logic if the enemy is not dead
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


    void HandleChase()
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

    void HandleIdle()
    {
        // Stop moving
        navMeshAgent.isStopped = true;

        // Set idle animation
        animator.SetBool("isWalking", false);

        // Reset attack trigger
        animator.ResetTrigger("AttackType1");
        animator.ResetTrigger("AttackType2");
    }

    void HandleAttack()
    {
        // Stop movement during attack
        navMeshAgent.isStopped = true;
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
}
