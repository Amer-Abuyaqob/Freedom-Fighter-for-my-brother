using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Collider2D hitboxCollider; // Trigger collider
    [SerializeField] LayerMask targetLayers;
    [SerializeField] int damage = 10;
    [SerializeField] float knockbackForce = 0f;

    readonly HashSet<Collider2D> alreadyHitThisSwing = new HashSet<Collider2D>();

    void Awake()
    {
        if (hitboxCollider == null) hitboxCollider = GetComponent<Collider2D>();
        if (hitboxCollider != null) hitboxCollider.enabled = false;
    }

    public void EnableHitbox()
    {
        if (hitboxCollider == null) return;
        alreadyHitThisSwing.Clear();
        hitboxCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        if (hitboxCollider == null) return;
        hitboxCollider.enabled = false;
        alreadyHitThisSwing.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hitboxCollider == null || !hitboxCollider.enabled) return;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        if (alreadyHitThisSwing.Contains(other)) return;
        alreadyHitThisSwing.Add(other);

        // Apply damage via IDamageable first
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        else
        {
            // Fallback: PlayerStats if present
            var stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
        }

        // Optional knockback
        if (knockbackForce > 0f)
        {
            var rb = other.attachedRigidbody;
            if (rb != null)
            {
                float direction = Mathf.Sign(other.transform.position.x - transform.position.x);
                rb.AddForce(new Vector2(direction * knockbackForce, knockbackForce * 0.3f), ForceMode2D.Impulse);
            }
        }
    }
}


