using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1f; // Maximum health
    [SerializeField] private FloatValueSO currentHealth; // ScriptableObject to store health value

    private void Start()
    {
        // Set the initial health to 1 (full health)
        currentHealth.Value = 0.2f;
    }

    public void Reduce(int damage)
    {
        // Reduce health and normalize it based on maxHealth
        currentHealth.Value -= damage / maxHealth;
        currentHealth.Value = Mathf.Clamp01(currentHealth.Value); // Ensure value is between 0 and 1
        
        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    public void AddHealth(float healthBoost)
{
    // Log the current health and boost being applied
    Debug.Log($"Adding health boost: {healthBoost}");

    // Directly add the health boost (already in the normalized [0,1] range)
    currentHealth.Value = Mathf.Clamp01(currentHealth.Value + healthBoost);

    // Log the new health value for debugging
    Debug.Log($"New health value: {currentHealth.Value}");
}


    private void Die()
    {
        Debug.Log("Character has died");
        currentHealth.Value = 0; // Set health to 0 on death
    }
}
