using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Drag this into the inspector
    public TextMeshProUGUI healthText; // Health text UI
    private EnemyHealth enemyHealth;
    
    private float targetHealth;
    private float smoothSpeed = 5f; // Speed of the transition

    void Start()
    {
        // Get the EnemyHealth component from the parent object
        enemyHealth = GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            healthSlider.maxValue = enemyHealth.maxHealth;
            healthSlider.value = enemyHealth.currentHealth;

            // Subscribe to health changes
            enemyHealth.OnHealthChanged += UpdateHealthBar;
            
            targetHealth = enemyHealth.currentHealth;
        }
        else
        {
            Debug.LogError("EnemyHealth not found on the parent object!");
        }
    }

    void Update()
    {
        // Smoothly transition the health value
        if (healthSlider.value != targetHealth)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, smoothSpeed * Time.deltaTime);
        }

        // Update text with lerp (rounded for readability)
        UpdateHealthText(Mathf.Round(healthSlider.value));
    }

    void UpdateHealthBar(float healthValue)
    {
        targetHealth = healthValue; // Set target value
    }

    void UpdateHealthText(float healthValue)
    {
        if (healthText != null)
        {
            healthText.text = $"{healthValue}/{enemyHealth.maxHealth}";
        }
    }
}
