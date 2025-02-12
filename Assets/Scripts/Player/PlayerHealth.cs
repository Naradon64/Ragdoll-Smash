using UnityEngine;
using System;
using UnityEngine.UI;  // For UI.Text or TextMeshProUGUI if using TMP
using TMPro;  // For TextMeshPro

public class PlayerHealth : MonoBehaviour
{
    private DieMenu dieMenu; // Reference to DieMenu
    public bool cursorLocked = true; // Track cursor lock state
    public float maxHealth = 100f;
    public float currentHealth;

    // Event to notify the UI when health changes
    public event Action<float> OnHealthChanged;

    public TextMeshProUGUI healthText;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);

        // Find the DieMenu script in the scene
        dieMenu = FindFirstObjectByType<DieMenu>();
        if (dieMenu == null)
        {
            Debug.LogError("DieMenu script not found in the scene!");
        }

        // Initialize health text display
        if (healthText != null)
        {
            UpdateHealthText();
        }
    }

    void Update()
    {   if (!PauseMenu.isPaused) 
        {
            // Debug: Press "E" to take 10 damage
            if (Input.GetKeyDown(KeyCode.G))
            {
                TakeDamage(10f);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player took {damage} damage! Current health: {currentHealth}/{maxHealth}");

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
        Debug.Log("Player Died!");
        // Handle player death (e.g., respawn, game over, etc.)
        if (dieMenu != null)
        {
            dieMenu.Die(); // Call DieMenu's Die function
        }
        else
        {
            Debug.LogError("DieMenu is not assigned in PlayerHealth.");
        }
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
