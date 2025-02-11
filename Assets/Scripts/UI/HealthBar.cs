using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Drag this into the inspector
    private PlayerHealth playerHealth;
    
    private float targetHealth;
    private float smoothSpeed = 5f; // Speed of the transition

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;

            // Subscribe to health changes
            playerHealth.OnHealthChanged += UpdateHealthBar;
            
            targetHealth = playerHealth.currentHealth;
        }
        else
        {
            Debug.LogError("PlayerHealth not found in the scene!");
        }
    }

    void Update()
    {
        // Smoothly transition the health value
        if (healthSlider.value != targetHealth)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, smoothSpeed * Time.deltaTime);
        }
    }

    void UpdateHealthBar(float healthValue)
    {
        targetHealth = healthValue; // Set target value
    }
}
