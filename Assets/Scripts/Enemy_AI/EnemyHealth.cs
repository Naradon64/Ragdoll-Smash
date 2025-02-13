using UnityEngine;
using System;
using TMPro; 

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public event Action<float> OnHealthChanged;
    public TextMeshProUGUI healthText;



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

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Enemy Died!");

        ///
        /// Code to trigger the ragdoll to "falling" state and then destroy the object
        ///

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
