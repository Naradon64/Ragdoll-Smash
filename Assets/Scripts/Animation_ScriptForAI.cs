using UnityEngine;
using UnityEngine.AI;

public class Animation_ScriptForAI : MonoBehaviour
{
    private Animator animator;         // Reference to the Animator component
    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent on the parent

    void Start()
    {
        // Get the Animator component on the current GameObject (subparent)
        animator = GetComponent<Animator>();

        // Get the NavMeshAgent component on the parent GameObject
        navMeshAgent = GetComponentInParent<NavMeshAgent>();

        // Debug to ensure components are properly set
        if (animator == null)
            Debug.LogError("Animator not found on Model!");

        if (navMeshAgent == null)
            Debug.LogError("NavMeshAgent not found on Parent!");
    }

    void Update()
    {
        if (navMeshAgent != null && animator != null)
        {
            // Check if the NavMeshAgent is moving
            bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;

            // Set the Animator boolean "isWalking" based on movement
            animator.SetBool("isWalking", isMoving);
        }
        else {
            animator.SetBool("isWalking", false);
        }
    }
}
