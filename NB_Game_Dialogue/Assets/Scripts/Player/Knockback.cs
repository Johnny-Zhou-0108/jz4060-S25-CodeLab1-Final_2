using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool gettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = 0.2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        // Use Rigidbody2D of the parent if it exists, otherwise use its own Rigidbody2D
        rb = transform.parent != null 
            ? transform.parent.GetComponent<Rigidbody2D>() 
            : GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody2D found for knockback.");
            return;
        }
        Debug.Log("Knockback!");

        gettingKnockedBack = true;

        // Calculate the knockback direction and apply force to the Rigidbody2D
        Vector2 difference = (transform.position - damageSource.position).normalized * knockBackThrust * rb.mass;
        rb.AddForce(difference, ForceMode2D.Impulse);

        // Start the knockback timer
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);

        // Stop the Rigidbody's velocity after knockback time
        rb.velocity = Vector2.zero;
        gettingKnockedBack = false;
    }
}
