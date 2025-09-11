using UnityEngine;

public class SpawnerSetup : MonoBehaviour
{
    [Header("Quick Setup")]
    [SerializeField] GameObject settler1Prefab;
    [SerializeField] GameObject settler2Prefab;
    
    [Header("Settler1 Limits")]
    [SerializeField] int settler1Capacity = 3;
    [SerializeField] int settler1Maximum = 10;
    
    [Header("Settler2 Limits")]
    [SerializeField] int settler2Capacity = 5;
    [SerializeField] int settler2Maximum = 15;
    
    [Header("Spawn Settings")]
    [SerializeField] float spawnInterval = 2f;
    
    [Header("Spawner Settings")]
    [SerializeField] float spawnRadius = 4f;
    [SerializeField] float minSpawnDistance = 8f;
    [SerializeField] bool destroyWhenComplete = true;
    
    [Header("Auto Setup")]
    [SerializeField] bool setupOnStart = true;
    
    EnemySpawner enemySpawner;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupSpawner();
        }
    }
    
    [ContextMenu("Setup Spawner")]
    public void SetupSpawner()
    {
        // Get or create EnemySpawner component
        enemySpawner = GetComponent<EnemySpawner>();
        if (enemySpawner == null)
        {
            enemySpawner = gameObject.AddComponent<EnemySpawner>();
        }
        
        // Clear existing variants
        enemySpawner.GetType().GetField("enemyVariants", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemySpawner, new System.Collections.Generic.List<EnemyVariant>());
        
        // Add Settler1 variant
        if (settler1Prefab != null)
        {
            AddEnemyVariant("Settler1", settler1Prefab, settler1Capacity, settler1Maximum, spawnInterval);
        }
        
        // Add Settler2 variant
        if (settler2Prefab != null)
        {
            AddEnemyVariant("Settler2", settler2Prefab, settler2Capacity, settler2Maximum, spawnInterval);
        }
        
        // Configure spawner settings
        ConfigureSpawnerSettings();
        
        Debug.Log($"SpawnerSetup: Configured spawner with Settler1 (Capacity: {settler1Capacity}, Maximum: {settler1Maximum}) and Settler2 (Capacity: {settler2Capacity}, Maximum: {settler2Maximum})");
    }
    
    void AddEnemyVariant(string name, GameObject prefab, int capacity, int maximum, float interval)
    {
        var variant = new EnemyVariant
        {
            variantName = name,
            enemyPrefab = prefab,
            capacity = capacity,
            maximum = maximum,
            spawnInterval = interval,
            currentAliveCount = 0,
            totalSpawnedCount = 0,
            lastSpawnTime = 0f,
            isActive = true,
            hasReachedCapacity = false,
            hasReachedMaximum = false
        };
        
        // Use reflection to add variant to the private list
        var variantsField = enemySpawner.GetType().GetField("enemyVariants", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (variantsField != null)
        {
            var variants = variantsField.GetValue(enemySpawner) as System.Collections.Generic.List<EnemyVariant>;
            if (variants != null)
            {
                variants.Add(variant);
            }
        }
    }
    
    void ConfigureSpawnerSettings()
    {
        // Use reflection to set private fields
        var spawnRadiusField = enemySpawner.GetType().GetField("spawnRadius", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        spawnRadiusField?.SetValue(enemySpawner, spawnRadius);
        
        var minDistanceField = enemySpawner.GetType().GetField("minSpawnDistance", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        minDistanceField?.SetValue(enemySpawner, minSpawnDistance);
        
        var destroyField = enemySpawner.GetType().GetField("destroyWhenAllMaximumReached", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        destroyField?.SetValue(enemySpawner, destroyWhenComplete);
    }
    
    // Public methods for runtime configuration
    public void SetSettler1Capacity(int capacity)
    {
        settler1Capacity = capacity;
        if (enemySpawner != null)
        {
            enemySpawner.SetVariantCapacity("Settler1", capacity);
        }
    }
    
    public void SetSettler1Maximum(int maximum)
    {
        settler1Maximum = maximum;
        if (enemySpawner != null)
        {
            enemySpawner.SetVariantMaximum("Settler1", maximum);
        }
    }
    
    public void SetSettler2Capacity(int capacity)
    {
        settler2Capacity = capacity;
        if (enemySpawner != null)
        {
            enemySpawner.SetVariantCapacity("Settler2", capacity);
        }
    }
    
    public void SetSettler2Maximum(int maximum)
    {
        settler2Maximum = maximum;
        if (enemySpawner != null)
        {
            enemySpawner.SetVariantMaximum("Settler2", maximum);
        }
    }
    
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
        // Note: This would require modifying the EnemySpawner to allow runtime interval changes
    }
    
    public void StartSpawner()
    {
        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning();
        }
    }
    
    public void StopSpawner()
    {
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }
    }
    
    void OnValidate()
    {
        // Ensure positive values
        settler1Capacity = Mathf.Max(1, settler1Capacity);
        settler1Maximum = Mathf.Max(1, settler1Maximum);
        settler2Capacity = Mathf.Max(1, settler2Capacity);
        settler2Maximum = Mathf.Max(1, settler2Maximum);
        spawnInterval = Mathf.Max(0.1f, spawnInterval);
        spawnRadius = Mathf.Max(0.1f, spawnRadius);
        minSpawnDistance = Mathf.Max(0.1f, minSpawnDistance);
        
        // Ensure capacity doesn't exceed maximum
        if (settler1Capacity > settler1Maximum)
            settler1Capacity = settler1Maximum;
        if (settler2Capacity > settler2Maximum)
            settler2Capacity = settler2Maximum;
    }
}
