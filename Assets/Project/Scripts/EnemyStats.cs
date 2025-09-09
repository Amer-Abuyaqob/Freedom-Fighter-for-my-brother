using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] int maxHealth = 50;

    int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth == 0)
        {
            HandleDeath();
        }
    }

    // TODO: Decide what to do upon killing enemy
    void HandleDeath()
    {
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isAlive", false);
        }

        Destroy(gameObject, 0.25f);
    }
}


