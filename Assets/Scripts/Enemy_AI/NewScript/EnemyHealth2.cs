using UnityEngine;
using System;
using TMPro;
using FIMSpace.FProceduralAnimation;
using System.Collections;
public class EnemyHealth2 : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public event Action<float> OnHealthChanged;
    public TextMeshProUGUI healthText;
    private RagdollAnimator2 myRagdollAnimator;
    private Canvas healthCanvas; 

    private Enemy_AI2 enemyAI;  // Reference to Enemy_AI2 component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);

        // Initialize health text display
        if (healthText != null)
        {
            UpdateHealthText();
        }

        healthCanvas = GetComponentInChildren<Canvas>();
        myRagdollAnimator = GetComponent<RagdollAnimator2>();

        // Get the Enemy_AI2 component
        enemyAI = GetComponent<Enemy_AI2>();
        if (enemyAI == null)
        {
            Debug.LogError("Enemy_AI2 component not found on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f, Vector3.forward); // Example of calling with direction
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float damage, Vector3 damageDirection)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Enemy took {damage} damage! Current health: {currentHealth}/{maxHealth}");

        // Notify the UI to update
        OnHealthChanged?.Invoke(currentHealth);

        // Apply knockback force
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            float upwardForce = 5f;   // Upward force
            float forwardForce = 100f;  // Forward push force
            float pushDelay = 0.1f;    // Delay between up and push

            // Step 1: Add upward force
            Vector3 upwardDirection = Vector3.up * upwardForce;
            rb.AddForce(upwardDirection, ForceMode.Impulse);

            // Start coroutine to add the push force after a delay
            StartCoroutine(AddPushForce(rb, forwardForce, pushDelay));
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        // Update health text UI
        if (healthText != null)
        {
            UpdateHealthText();
        }
    }

    // Coroutine to add the push force with delay
    private IEnumerator AddPushForce(Rigidbody rb, float forwardForce, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Step 2: Add push force (backward/forward)
        Vector3 pushDirection = -transform.forward * forwardForce;
        rb.AddForce(pushDirection, ForceMode.Impulse);
    }

    void Die()
    {
        Debug.Log("Enemy Died!");

        // Access the Animator component and set 'isDead' to true
        Animator enemyAnimator = GetComponent<Animator>();
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("isDead", true);  // Set isDead flag
        }
        if (healthCanvas != null)
        {
            Invoke(nameof(DisableHealthCanvas), 3f);
        }

        // Trigger the ragdoll fall state
        // myRagdollAnimator.User_SwitchFallState();

        // Destroy the enemy after a 5-second delay
        // Invoke("Destroy", 10f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    void DisableHealthCanvas()
    {
        healthCanvas.gameObject.SetActive(false);
    }

    // Helper method to update the health text
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }
}
