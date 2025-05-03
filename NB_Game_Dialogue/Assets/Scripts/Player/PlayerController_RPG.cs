using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController_RPG : MonoBehaviour
{
    [Header("Movement Params")]
    public float moveSpeed = 6.0f;

    // Components
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasStopped = false;

    // Layers for collision detection
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0; // Disable gravity in a top-down RPG
    }

    private void FixedUpdate()
    {
        // Check if dialogue is playing, pause movement if true
        if (DialogueManager_Modified.GetInstance().dialogueIsPlaying)
        {
            StopMovement();
            return;
        }

        // If attacking, block movement
        if (animator.GetBool("isAttacking"))
        {
            StopMovement();
            return;
        }

        // Handle Movement
        Vector2 moveDirection = InputManager.GetInstance().GetMoveDirection();

        if (moveDirection != Vector2.zero)
        {
            Vector3 targetPos = transform.position + new Vector3(moveDirection.x, moveDirection.y, 0) * Time.fixedDeltaTime * moveSpeed;

            if (IsWalkable(targetPos))
            {
                HandleMovement(moveDirection);
                hasStopped = false;
            }
            else if (!hasStopped)
            {
                StopMovement();
            }
        }
        else
        {
            StopMovement();
        }

        // Handle Attack
        if (InputManager.GetInstance().GetAttackPressed())
        {
            Debug.Log("Attack initiated!");
            HandleAttack();
        }
    }

    private void HandleMovement(Vector2 moveDirection)
    {
        // Constrain movement to one axis at a time
        if (moveDirection.x != 0)
        {
            moveDirection.y = 0;
        }

        // Apply movement to the Rigidbody
        rb.velocity = moveDirection * moveSpeed;

        // Update animator parameters
        animator.SetFloat("moveX", moveDirection.x);
        animator.SetFloat("moveY", moveDirection.y);
        animator.SetBool("isMoving", true);
    }

    private void StopMovement()
    {
        // Stop the Rigidbody's movement
        rb.velocity = Vector2.zero;

        // Reset animator state
        animator.SetBool("isMoving", false);

        hasStopped = true;
    }

    private void HandleAttack()
    {
        // Trigger the attack animation
        animator.SetBool("isAttacking", true);

        // Determine the attack direction based on the player's facing direction
        float moveX = animator.GetFloat("moveX");
        float moveY = animator.GetFloat("moveY");

        if (moveY > 0)
        {
            animator.Play("Attack_Up");
        }
        else if (moveY < 0)
        {
            animator.Play("Attack_Down");
        }
        else if (moveX > 0)
        {
            animator.Play("Attack_Right");
        }
        else if (moveX < 0)
        {
            animator.Play("Attack_Left");
        }

        // Reset the attacking state after the animation finishes
        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        // Adjust delay based on the length of your attack animation
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isAttacking", false);
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        // Check if the target position overlaps with a solid object
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) == null;
    }
}
