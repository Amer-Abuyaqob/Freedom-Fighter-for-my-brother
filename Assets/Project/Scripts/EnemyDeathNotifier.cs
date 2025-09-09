using UnityEngine;

public class EnemyDeathNotifier : MonoBehaviour
{
    void Start()
    {
        // Subscribe to enemy death
        var enemyStats = GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            // We'll need to modify EnemyStats to fire an event
            // For now, we'll check in Update (simple approach)
        }
    }

    void Update()
    {
        var enemyStats = GetComponent<EnemyStats>();
        if (enemyStats != null && enemyStats.CurrentHealth <= 0)
        {
            // Notify HUD that this enemy died
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.OnEnemyKilled();
            }
            
            // Destroy this component to prevent multiple notifications
            Destroy(this);
        }
    }
}
