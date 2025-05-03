using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private float restoreDefaultColorTime = 0.2f; // Duration of the flash

    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator FlashRoutine()
    {
        // Change the sprite color to red
        spriteRenderer.color = Color.red;

        // Wait for the specified duration
        yield return new WaitForSeconds(restoreDefaultColorTime);

        // Change the sprite color back to white
        spriteRenderer.color = Color.white;

        // Check for death after the flash
        enemyHealth.DetectDeath();
    }
}
