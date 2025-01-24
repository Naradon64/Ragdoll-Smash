using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 10f;  // Range to detect the player
    public float attackRange = 1.5f;    // Range to perform attacks
    public float attackCooldown = 1.5f; // Time between attacks

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
    }

    void Update()
    {
        // Calculate distance to the player
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check for attack or movement
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

        // Check if enough time has passed since the last attack
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Start the attack animation (randomize between two attack types)
            int attackType = Random.Range(0, 2);
            if (attackType == 0)
            {
                animator.SetTrigger("AttackType1");
            }
            else
            {
                animator.SetTrigger("AttackType2");
            }

            // Update last attack time
            lastAttackTime = Time.time;
        }
    }
}
