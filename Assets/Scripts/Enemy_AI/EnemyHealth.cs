using UnityEngine;
using System;
using TMPro;
using FIMSpace.FProceduralAnimation;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public event Action<float> OnHealthChanged;
    public TextMeshProUGUI healthText;
    public RagdollAnimator2 myRagdollAnimator;
    

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            {
                TakeDamage(10f);
            }
         if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Enemy took {damage} damage! Current health: {currentHealth}/{maxHealth}");

        // Notify the UI to update
        OnHealthChanged?.Invoke(currentHealth);

        // Update health text UI
        if (healthText != null)
        {
            UpdateHealthText();
        }
    }
    void Die()
    {
        Debug.Log("Enemy Died!");

        // Access the Animator component and set 'isDead' to true
        Animator enemyAnimator = GetComponentInChildren<Animator>();
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("isDead", true);  // Set isDead flag
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



    // Helper method to update the health text
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }
}
