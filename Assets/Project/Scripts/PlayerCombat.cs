using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);
}

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;

    [Header("Attack Settings")]
    [SerializeField] Transform attackPoint; // legacy fallback
    [Header("Attack Points")]
    [SerializeField] Transform punchPoint;
    [SerializeField] Transform kickPoint;
    [SerializeField] float attackRange = 0.4f;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] int punchDamage = 10;
    [SerializeField] int kickDamage = 15;
    [SerializeField] float attackCooldown = 0.25f;

    [Header("Animation State Names")]
    [SerializeField] string punchStateName = "punching";
    [SerializeField] string kickStateName = "kicking";

    [Header("Timing")]
    [SerializeField] float contactDelay = 0.1f; // delay from state start to apply hit

    bool isAttacking = false;
    float nextAttackTime = 0f;

    public bool IsAttacking => isAttacking;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        // Migrate legacy single point into specific points when provided
        if (punchPoint == null && attackPoint != null) punchPoint = attackPoint;
        if (kickPoint == null && attackPoint != null) kickPoint = attackPoint;
        if (punchPoint == null) punchPoint = this.transform;
        if (kickPoint == null) kickPoint = this.transform;
    }

    void Update()
    {
        // Block inputs while attacking
        if (isAttacking) return;
        if (Time.time < nextAttackTime) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(PerformAttack("Punch", punchDamage, punchPoint, punchStateName));
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(PerformAttack("Kick", kickDamage, kickPoint, kickStateName));
        }
    }

    IEnumerator PerformAttack(string triggerName, int damage, Transform point, string stateName)
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        // Wait until we enter the expected animation state
        if (animator != null && !string.IsNullOrEmpty(stateName))
        {
            while (!IsInState(animator, 0, stateName))
            {
                yield return null;
            }
        }

        // Optional contact delay to match the contact frame
        if (contactDelay > 0f)
            yield return new WaitForSeconds(contactDelay);

        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(point.position, attackRange, enemyLayers);
        foreach (var collider in hitEnemies)
        {
            var damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            
            // FIXME: Might be unnecessary
            else
            {
                // Fallback for objects using PlayerStats or similar
                var stats = collider.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damage);
                }
            }
        }

        // Wait for the state to finish before unlocking
        if (animator != null && !string.IsNullOrEmpty(stateName))
        {
            while (IsInState(animator, 0, stateName) && !HasStateFinished(animator, 0))
            {
                yield return null;
            }
        }

        isAttacking = false;
    }

    bool IsInState(Animator anim, int layer, string stateName)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(layer);
        return info.IsName(stateName);
    }

    bool HasStateFinished(Animator anim, int layer)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(layer);
        return info.normalizedTime >= 1f && !anim.IsInTransition(layer);
    }

    // FIXME: Might be unnecessary
    void OnDrawGizmosSelected()
    {
        // Draw punch point
        if (punchPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(punchPoint.position, attackRange);
        }
        // Draw kick point
        if (kickPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(kickPoint.position, attackRange);
        }
    }
}


