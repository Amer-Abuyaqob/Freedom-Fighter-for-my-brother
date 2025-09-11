using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyVariant
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public string variantName = "Enemy";
    
    [Header("Capacity Limits")]
    [Tooltip("Maximum number of this variant alive at the same time")]
    public int capacity = 3;
    public int currentAliveCount = 0;
    
    [Header("Maximum Limits")]
    [Tooltip("Total number of this variant to spawn in the level")]
    public int maximum = 10;
    public int totalSpawnedCount = 0;
    
    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float lastSpawnTime = 0f;
    
    [Header("Status")]
    public bool isActive = true;
    public bool hasReachedCapacity = false;
    public bool hasReachedMaximum = false;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] List<EnemyVariant> enemyVariants = new List<EnemyVariant>();
    [SerializeField] float spawnRadius = 3f;
    [SerializeField] float minSpawnDistance = 8f; // Distance from player to prevent spawning
    [SerializeField] bool spawnOnStart = true;
    [SerializeField] float initialDelay = 1f;
    
    [Header("Spawner Behavior")]
    [SerializeField] bool destroyWhenAllMaximumReached = true;
    [SerializeField] bool showSpawnRadius = true;
    [SerializeField] Color spawnRadiusColor = Color.green;
    [SerializeField] Color disabledRadiusColor = Color.red;
    
    [Header("Debug")]
    [SerializeField] bool enableDebugLog = false;
    
    Transform player;
    bool isSpawning = false;
    bool allVariantsReachedMax = false;
    Coroutine spawningCoroutine;
    
    void Awake()
    {
        // Find player
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        // Initialize spawn times
        foreach (var variant in enemyVariants)
        {
            variant.lastSpawnTime = Time.time;
        }
    }
    
    void Start()
    {
        if (spawnOnStart)
        {
            StartSpawning();
        }
    }
    
    void Update()
    {
        // Update current counts by counting active enemies
        UpdateCurrentCounts();
        
        // Check if all variants have reached their maximum
        CheckAllVariantsReachedMaximum();
    }
    
    public void StartSpawning()
    {
        if (isSpawning) return;
        
        if (enableDebugLog) Debug.Log($"EnemySpawner: Starting spawner on {gameObject.name}");
        
        isSpawning = true;
        spawningCoroutine = StartCoroutine(SpawnEnemiesCoroutine());
    }
    
    public void StopSpawning()
    {
        if (!isSpawning) return;
        
        if (enableDebugLog) Debug.Log($"EnemySpawner: Stopping spawner on {gameObject.name}");
        
        isSpawning = false;
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null;
        }
    }
    
    IEnumerator SpawnEnemiesCoroutine()
    {
        // Initial delay
        yield return new WaitForSeconds(initialDelay);
        
        while (isSpawning && !allVariantsReachedMax)
        {
            // Try to spawn each variant
            foreach (var variant in enemyVariants)
            {
                if (CanSpawnVariant(variant))
                {
                    TrySpawnVariant(variant);
                }
            }
            
            // Wait before next spawn attempt
            yield return new WaitForSeconds(0.5f);
        }
        
        if (enableDebugLog) Debug.Log($"EnemySpawner: Spawning stopped on {gameObject.name}");
    }
    
    bool CanSpawnVariant(EnemyVariant variant)
    {
        // Check if variant is active
        if (!variant.isActive)
            return false;
        
        // Check global level limits first
        if (LevelManager.Instance != null)
        {
            if (!LevelManager.Instance.CanSpawnVariant(variant.variantName))
                return false;
        }
        
        // Check spawn interval
        if (Time.time - variant.lastSpawnTime < variant.spawnInterval)
            return false;
        
        // Check if player is too close
        if (IsPlayerTooClose())
            return false;
        
        return true;
    }
    
    void TrySpawnVariant(EnemyVariant variant)
    {
        if (variant.enemyPrefab == null)
        {
            if (enableDebugLog) Debug.LogWarning($"EnemySpawner: No prefab assigned for variant {variant.variantName}");
            return;
        }
        
        // Calculate spawn position
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // Spawn the enemy
        GameObject spawnedEnemy = Instantiate(variant.enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Update variant data
        variant.totalSpawnedCount++;
        variant.currentAliveCount++;
        variant.lastSpawnTime = Time.time;
        
        // Notify LevelManager of the spawn
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnEnemySpawned(variant.variantName);
        }
        
        // Add EnemyDeathNotifier to track when enemy dies
        var deathNotifier = spawnedEnemy.GetComponent<EnemyDeathNotifier>();
        if (deathNotifier == null)
        {
            deathNotifier = spawnedEnemy.AddComponent<EnemyDeathNotifier>();
        }
        
        // Subscribe to enemy death to update count
        StartCoroutine(WaitForEnemyDeath(spawnedEnemy, variant));
        
        if (enableDebugLog) Debug.Log($"EnemySpawner: Spawned {variant.variantName} at {spawnPosition}. Local: {variant.currentAliveCount} alive, {variant.totalSpawnedCount} total");
    }
    
    IEnumerator WaitForEnemyDeath(GameObject enemy, EnemyVariant variant)
    {
        // Wait until enemy is destroyed
        while (enemy != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Enemy died, decrement alive count
        variant.currentAliveCount = Mathf.Max(0, variant.currentAliveCount - 1);
        
        // Notify LevelManager of the death
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnEnemyDied(variant.variantName);
        }
        
        if (enableDebugLog) Debug.Log($"EnemySpawner: {variant.variantName} died. Local: {variant.currentAliveCount} alive, {variant.totalSpawnedCount} total");
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition;
        int attempts = 0;
        int maxAttempts = 10;
        
        do
        {
            // Generate random position within spawn radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            spawnPosition = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);
            
            attempts++;
        }
        while (attempts < maxAttempts && IsPlayerTooClose(spawnPosition));
        
        return spawnPosition;
    }
    
    bool IsPlayerTooClose(Vector3 position)
    {
        if (player == null) return false;
        return Vector3.Distance(position, player.position) < minSpawnDistance;
    }
    
    bool IsPlayerTooClose()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) < minSpawnDistance;
    }
    
    void UpdateCurrentCounts()
    {
        // Count active enemies of each variant (for local tracking)
        foreach (var variant in enemyVariants)
        {
            if (variant.enemyPrefab != null)
            {
                int activeCount = 0;
                var enemies = FindObjectsOfType<EnemyStats>();
                
                foreach (var enemy in enemies)
                {
                    // Check if this enemy matches the variant (by name or component)
                    if (enemy.name.Contains(variant.variantName) || 
                        enemy.gameObject.name.Contains(variant.enemyPrefab.name))
                    {
                        activeCount++;
                    }
                }
                
                variant.currentAliveCount = activeCount;
            }
        }
    }
    
    void CheckAllVariantsReachedMaximum()
    {
        // Check if level is complete (handled by LevelManager)
        if (LevelManager.Instance != null && LevelManager.Instance.IsLevelComplete())
        {
            if (!allVariantsReachedMax)
            {
                allVariantsReachedMax = true;
                StopSpawning();
                
                if (enableDebugLog) Debug.Log($"EnemySpawner: Level complete, stopping spawner {gameObject.name}");
            }
        }
    }
    
    IEnumerator DestroySpawnerAfterDelay()
    {
        yield return new WaitForSeconds(2f); // Wait a bit before destroying
        
        if (enableDebugLog) Debug.Log($"EnemySpawner: Destroying spawner {gameObject.name}");
        
        Destroy(gameObject);
    }
    
    // Public methods for external control
    public void AddEnemyVariant(EnemyVariant newVariant)
    {
        enemyVariants.Add(newVariant);
    }
    
    public void RemoveEnemyVariant(string variantName)
    {
        enemyVariants.RemoveAll(v => v.variantName == variantName);
    }
    
    public void SetVariantCapacity(string variantName, int newCapacity)
    {
        var variant = enemyVariants.Find(v => v.variantName == variantName);
        if (variant != null)
        {
            variant.capacity = newCapacity;
        }
    }
    
    public void SetVariantMaximum(string variantName, int newMaximum)
    {
        var variant = enemyVariants.Find(v => v.variantName == variantName);
        if (variant != null)
        {
            variant.maximum = newMaximum;
        }
    }
    
    public void SetVariantActive(string variantName, bool active)
    {
        var variant = enemyVariants.Find(v => v.variantName == variantName);
        if (variant != null)
        {
            variant.isActive = active;
        }
    }
    
    public int GetVariantCurrentAliveCount(string variantName)
    {
        var variant = enemyVariants.Find(v => v.variantName == variantName);
        return variant != null ? variant.currentAliveCount : 0;
    }
    
    public int GetVariantCapacity(string variantName)
    {
        var variant = enemyVariants.Find(v => v.variantName == variantName);
        return variant != null ? variant.capacity : 0;
    }
    
    public int GetVariantTotalSpawned(string variantName)
    {
        var variant = enemyVariants.Find(v => v.variantName == variantName);
        return variant != null ? variant.totalSpawnedCount : 0;
    }
    
    public int GetVariantMaximum(string variantName)
    {
        var variant = enemyVariants.Find(v => v.variantName == variantName);
        return variant != null ? variant.maximum : 0;
    }
    
    public bool IsAllVariantsReachedMax()
    {
        return allVariantsReachedMax;
    }
    
    void OnDrawGizmos()
    {
        if (showSpawnRadius)
        {
            Gizmos.color = IsPlayerTooClose() ? disabledRadiusColor : spawnRadiusColor;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
            
            // Draw minimum spawn distance
            Gizmos.color = Color.yellow;
            if (player != null)
            {
                Gizmos.DrawWireSphere(player.position, minSpawnDistance);
            }
        }
    }
    
    void OnValidate()
    {
        // Ensure positive values
        spawnRadius = Mathf.Max(0.1f, spawnRadius);
        minSpawnDistance = Mathf.Max(0.1f, minSpawnDistance);
        initialDelay = Mathf.Max(0f, initialDelay);
        
        // Validate enemy variants
        foreach (var variant in enemyVariants)
        {
            variant.capacity = Mathf.Max(1, variant.capacity);
            variant.maximum = Mathf.Max(1, variant.maximum);
            variant.spawnInterval = Mathf.Max(0.1f, variant.spawnInterval);
            variant.currentAliveCount = Mathf.Max(0, variant.currentAliveCount);
            variant.totalSpawnedCount = Mathf.Max(0, variant.totalSpawnedCount);
            
            // Ensure capacity doesn't exceed maximum
            if (variant.capacity > variant.maximum)
            {
                variant.capacity = variant.maximum;
            }
        }
    }
}
