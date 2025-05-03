using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private float knockBackThrust = 5f; // Knockback strength
    private int currentHealth;
    private bool hasTakenDamage = false;
    private Flash flash;
    private Animator animator; // Reference to the Animator

    private void Awake()
    {
        flash = GetComponent<Flash>();
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    private void Start()
    {
        currentHealth = startingHealth;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Animator playerAnimator = other.GetComponent<Animator>();
            if (playerAnimator != null && playerAnimator.GetBool("isAttacking") && !hasTakenDamage)
            {
                // Player successfully attacked the enemy
                TakeDamage(1);
                ApplyKnockback(other.transform);

                // Trigger camera shake
                CameraShake cameraShake = other.GetComponentInChildren<CameraShake>();
                if (cameraShake != null)
                {
                    cameraShake.TriggerShake();
                }

                hasTakenDamage = true;
                Invoke(nameof(ResetDamageFlag), 0.5f);
            }
        }
    }

    private void ResetDamageFlag()
    {
        hasTakenDamage = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(flash.FlashRoutine());
        DetectDeath();
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            animator.SetBool("IsDead", true); // Trigger the death animation
            // Wait for the death animation to finish before destroying the object
            StartCoroutine(HandleDeath());
        }
    }

    private System.Collections.IEnumerator HandleDeath()
    {
        // Wait for the death animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Destroy the game object after the animation finishes
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ApplyKnockback(Transform damageSource)
    {
        Vector2 knockbackDirection = (transform.position - damageSource.position).normalized;
        float knockbackSpeed = knockBackThrust;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.Knockback(knockbackDirection, knockbackSpeed);
        }
    }
}
