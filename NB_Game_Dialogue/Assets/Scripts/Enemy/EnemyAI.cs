using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State {
        Roaming,
        KnockedBack
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;
    private Vector2 knockbackDirection; // Direction of knockback
    private float knockbackSpeed; // Speed of knockback
    private bool isKnockedBack = false;

    [SerializeField] private float knockbackDuration = 0.2f; // Duration of knockback

    private void Awake() {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
    }

    private void Start() {
        StartCoroutine(RoamingRoutine());
    }

    private IEnumerator RoamingRoutine() {
        while (true) {
            if (state == State.Roaming) {
                Vector2 roamPosition = GetRoamingPosition();
                enemyPathfinding.MoveTo(roamPosition);
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition() {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public void Knockback(Vector2 direction, float speed) {
        if (state != State.KnockedBack) {
            Debug.Log($"Knockback started: Direction = {direction}, Speed = {speed}");
            knockbackDirection = direction;
            knockbackSpeed = speed;
            isKnockedBack = true;
            StartCoroutine(KnockbackRoutine());
        }
    }

    private IEnumerator KnockbackRoutine() {
        state = State.KnockedBack;

        float elapsedTime = 0f;
        while (elapsedTime < knockbackDuration) {
            // Move the enemy in the knockback direction
            transform.position += (Vector3)knockbackDirection * knockbackSpeed * Time.deltaTime;
            Debug.Log($"Knockback in progress: Position = {transform.position}");
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Knockback finished");
        isKnockedBack = false;
        state = State.Roaming; // Return to roaming state
    }
}
