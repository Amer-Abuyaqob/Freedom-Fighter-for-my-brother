using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Item
{
    [Header("Healing Settings")]
    public int healAmount = 25;
    
    [Header("Capacity Management")]
    public string variantName = "HealthItem"; // Should match HealerCapacityManager variant name

     public override void OnPickup(GameObject collector)
    {
        base.OnPickup(collector); // Play sound from parent

        // Apply healing
        PlayerStats stats = collector.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.AddHealth(healAmount);
        }
        
        // Notify capacity manager that this healer was destroyed
        if (HealerCapacityManager.Instance != null)
        {
            HealerCapacityManager.Instance.OnHealerDestroyed(variantName);
        }
    }
}
