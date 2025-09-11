using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealerVariantLimit
{
    [Header("Healer Variant")]
    public string variantName = "HealthItem";
    public GameObject healerPrefab;
    
    [Header("Capacity Limits")]
    [Tooltip("Maximum number of this variant alive in the level")]
    public int maxCapacity = 3;
    public int currentAliveCount = 0;
    
    [Header("Status")]
    public bool hasReachedCapacity = false;
}

public class HealerCapacityManager : MonoBehaviour
{
    [Header("Healer Capacity Limits")]
    [SerializeField] List<HealerVariantLimit> healerLimits = new List<HealerVariantLimit>();
    
    [Header("Debug")]
    [SerializeField] bool enableDebugLog = false;
    [SerializeField] bool showCapacityCounts = true;
    
    public static HealerCapacityManager Instance { get; private set; }
    
    void Awake()
    {
        // Simple singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Initialize all limits
        InitializeLimits();
        
        if (enableDebugLog)
        {
            Debug.Log($"HealerCapacityManager: Initialized with {healerLimits.Count} healer variant limits");
        }
    }
    
    void Update()
    {
        // Update current counts
        UpdateCurrentCounts();
    }
    
    void InitializeLimits()
    {
        // Initialize all healer limits
        foreach (var limit in healerLimits)
        {
            limit.currentAliveCount = 0;
            limit.hasReachedCapacity = false;
        }
    }
    
    void UpdateCurrentCounts()
    {
        // Count all healers of each variant in the level
        foreach (var limit in healerLimits)
        {
            if (limit.healerPrefab != null)
            {
                int aliveCount = 0;
                var healers = FindObjectsOfType<Healer>();
                
                foreach (var healer in healers)
                {
                    // Check if this healer matches the variant
                    if (healer.name.Contains(limit.variantName) || 
                        healer.gameObject.name.Contains(limit.healerPrefab.name))
                    {
                        aliveCount++;
                    }
                }
                
                limit.currentAliveCount = aliveCount;
                
                // Update capacity status
                if (limit.currentAliveCount >= limit.maxCapacity)
                {
                    limit.hasReachedCapacity = true;
                }
                else
                {
                    limit.hasReachedCapacity = false;
                }
            }
        }
    }
    
    // Public methods for spawners to check capacity
    public bool CanSpawnHealer(string variantName)
    {
        var limit = healerLimits.Find(l => l.variantName == variantName);
        if (limit == null) return false;
        
        // Check if capacity is reached
        if (limit.hasReachedCapacity)
        {
            if (enableDebugLog)
            {
                Debug.Log($"HealerCapacityManager: Capacity reached for {variantName}. Cannot spawn.");
            }
            return false;
        }
        
        return true;
    }
    
    public void OnHealerSpawned(string variantName)
    {
        var limit = healerLimits.Find(l => l.variantName == variantName);
        if (limit != null)
        {
            limit.currentAliveCount++;
            
            // Check if capacity is reached
            if (limit.currentAliveCount >= limit.maxCapacity)
            {
                limit.hasReachedCapacity = true;
                
                if (enableDebugLog)
                {
                    Debug.Log($"HealerCapacityManager: Capacity reached for {variantName} ({limit.maxCapacity}). Pausing spawning of this variant.");
                }
            }
            
            if (enableDebugLog)
            {
                Debug.Log($"HealerCapacityManager: {variantName} spawned. Current: {limit.currentAliveCount}/{limit.maxCapacity}");
            }
        }
    }
    
    public void OnHealerDestroyed(string variantName)
    {
        var limit = healerLimits.Find(l => l.variantName == variantName);
        if (limit != null)
        {
            limit.currentAliveCount = Mathf.Max(0, limit.currentAliveCount - 1);
            
            // Check if capacity is freed
            if (limit.hasReachedCapacity && limit.currentAliveCount < limit.maxCapacity)
            {
                limit.hasReachedCapacity = false;
                
                if (enableDebugLog)
                {
                    Debug.Log($"HealerCapacityManager: Capacity freed for {variantName}. Spawning can resume.");
                }
            }
            
            if (enableDebugLog)
            {
                Debug.Log($"HealerCapacityManager: {variantName} destroyed. Current: {limit.currentAliveCount}/{limit.maxCapacity}");
            }
        }
    }
    
    // Public getters for external systems
    public int GetCurrentCount(string variantName)
    {
        var limit = healerLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.currentAliveCount : 0;
    }
    
    public int GetMaxCapacity(string variantName)
    {
        var limit = healerLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.maxCapacity : 0;
    }
    
    public bool IsCapacityReached(string variantName)
    {
        var limit = healerLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.hasReachedCapacity : false;
    }
    
    // Method to add healer limits at runtime
    public void AddHealerLimit(string variantName, GameObject prefab, int maxCapacity)
    {
        var existingLimit = healerLimits.Find(l => l.variantName == variantName);
        if (existingLimit == null)
        {
            var newLimit = new HealerVariantLimit
            {
                variantName = variantName,
                healerPrefab = prefab,
                maxCapacity = maxCapacity,
                currentAliveCount = 0,
                hasReachedCapacity = false
            };
            
            healerLimits.Add(newLimit);
            
            if (enableDebugLog)
            {
                Debug.Log($"HealerCapacityManager: Added healer limit for {variantName} (Capacity: {maxCapacity})");
            }
        }
    }
    
    void OnGUI()
    {
        if (showCapacityCounts && enableDebugLog)
        {
            GUILayout.BeginArea(new Rect(10, 220, 300, 150));
            GUILayout.Label("Healer Capacity Counts:", GUI.skin.box);
            
            foreach (var limit in healerLimits)
            {
                string status = limit.hasReachedCapacity ? " [FULL]" : "";
                GUILayout.Label($"{limit.variantName}: {limit.currentAliveCount}/{limit.maxCapacity}{status}");
            }
            
            GUILayout.EndArea();
        }
    }
    
    void OnValidate()
    {
        // Ensure positive values
        foreach (var limit in healerLimits)
        {
            limit.maxCapacity = Mathf.Max(1, limit.maxCapacity);
        }
    }
}
