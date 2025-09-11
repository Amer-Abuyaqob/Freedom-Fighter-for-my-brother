using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GlobalEnemyLimit
{
    [Header("Enemy Variant")]
    public string variantName = "Enemy";
    public GameObject enemyPrefab;
    
    [Header("Global Level Limits")]
    [Tooltip("Maximum number of this variant alive across ALL spawners")]
    public int globalCapacity = 5;
    public int currentGlobalAliveCount = 0;
    
    [Tooltip("Total number of this variant to spawn across ALL spawners in the level")]
    public int globalMaximum = 20;
    public int totalGlobalSpawnedCount = 0;
    
    [Header("Status")]
    public bool hasReachedGlobalCapacity = false;
    public bool hasReachedGlobalMaximum = false;
}

public class LevelManager : MonoBehaviour
{
    [Header("Global Level Limits")]
    [SerializeField] List<GlobalEnemyLimit> globalLimits = new List<GlobalEnemyLimit>();
    
    [Header("Level Settings")]
    [SerializeField] bool destroyAllSpawnersWhenComplete = true;
    [SerializeField] float completionDelay = 2f;
    [SerializeField] string outroSceneName = "Outro_Lv1";
    
    [Header("Debug")]
    [SerializeField] bool enableDebugLog = false;
    [SerializeField] bool showGlobalCounts = true;
    
    List<EnemySpawner> allSpawners = new List<EnemySpawner>();
    bool levelComplete = false;
    bool spawnersDestroyed = false;
    
    public static LevelManager Instance { get; private set; }
    
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
        // Find all spawners in the scene
        FindAllSpawners();
        
        // Initialize global limits
        InitializeGlobalLimits();
        
        if (enableDebugLog)
        {
            Debug.Log($"LevelManager: Initialized with {globalLimits.Count} global enemy limits");
        }
    }
    
    void Update()
    {
        // Update global counts
        UpdateGlobalCounts();
        
        // Check if all variants have reached maximum (destroy spawners)
        CheckAllVariantsReachedMaximum();
        
        // Check if level is complete
        CheckLevelCompletion();
    }
    
    void FindAllSpawners()
    {
        allSpawners.Clear();
        var spawners = FindObjectsOfType<EnemySpawner>();
        
        foreach (var spawner in spawners)
        {
            allSpawners.Add(spawner);
        }
        
        if (enableDebugLog)
        {
            Debug.Log($"LevelManager: Found {allSpawners.Count} spawners in scene");
        }
    }
    
    void InitializeGlobalLimits()
    {
        // Initialize all global limits
        foreach (var limit in globalLimits)
        {
            limit.currentGlobalAliveCount = 0;
            limit.totalGlobalSpawnedCount = 0;
            limit.hasReachedGlobalCapacity = false;
            limit.hasReachedGlobalMaximum = false;
        }
    }
    
    void UpdateGlobalCounts()
    {
        // Count all enemies of each variant across the entire level
        foreach (var limit in globalLimits)
        {
            if (limit.enemyPrefab != null)
            {
                int aliveCount = 0;
                var enemies = FindObjectsOfType<EnemyStats>();
                
                foreach (var enemy in enemies)
                {
                    // Check if this enemy matches the variant
                    if (enemy.name.Contains(limit.variantName) || 
                        enemy.gameObject.name.Contains(limit.enemyPrefab.name))
                    {
                        aliveCount++;
                    }
                }
                
                limit.currentGlobalAliveCount = aliveCount;
                
                // Update global capacity status
                if (limit.currentGlobalAliveCount >= limit.globalCapacity)
                {
                    limit.hasReachedGlobalCapacity = true;
                }
                else
                {
                    limit.hasReachedGlobalCapacity = false;
                }
            }
        }
    }
    
    void CheckAllVariantsReachedMaximum()
    {
        if (spawnersDestroyed) return;
        
        // Check if all variants have reached their global maximum
        bool allReachedMaximum = true;
        foreach (var limit in globalLimits)
        {
            if (!limit.hasReachedGlobalMaximum)
            {
                allReachedMaximum = false;
                break;
            }
        }
        
        // If all variants have reached maximum, destroy spawners
        if (allReachedMaximum && destroyAllSpawnersWhenComplete)
        {
            spawnersDestroyed = true;
            StartCoroutine(DestroyAllSpawnersAfterDelay());
            
            if (enableDebugLog)
            {
                Debug.Log("LevelManager: All variants reached global maximum. Destroying spawners.");
            }
        }
    }
    
    void CheckLevelCompletion()
    {
        if (levelComplete) return;
        
        // Check if all variants have reached their global maximum (all enemies spawned)
        bool allReachedMaximum = true;
        foreach (var limit in globalLimits)
        {
            if (!limit.hasReachedGlobalMaximum)
            {
                allReachedMaximum = false;
                break;
            }
        }
        
        // If all variants have reached maximum AND all enemies are killed, level is complete
        if (allReachedMaximum && AreAllEnemiesKilled())
        {
            OnLevelComplete();
        }
    }
    
    bool AreAllEnemiesKilled()
    {
        // Check if all enemies of all variants have been killed
        foreach (var limit in globalLimits)
        {
            // If we've spawned the maximum but there are still alive enemies, not all are killed
            if (limit.hasReachedGlobalMaximum && limit.currentGlobalAliveCount > 0)
            {
                return false;
            }
        }
        
        return true;
    }
    
    void OnLevelComplete()
    {
        levelComplete = true;
        
        if (enableDebugLog)
        {
            Debug.Log("LevelManager: Level completed! All enemies spawned and killed.");
        }
        
        // Spawners should already be destroyed when all variants reached maximum
        // Only destroy them now if they weren't destroyed before
        if (destroyAllSpawnersWhenComplete && !spawnersDestroyed)
        {
            DestroyAllSpawnersImmediately();
        }
        
        // Notify other systems
        if (HUDManager.Instance != null)
        {
            // You could add a level completion method to HUDManager
            // HUDManager.Instance.OnLevelCompleted();
        }
        
        // Let VictoryManager handle the victory sequence and transition
        if (VictoryManager.Instance != null)
        {
            VictoryManager.Instance.TriggerVictorySequence();
        }
        else
        {
            // Fallback: transition directly if no VictoryManager
            if (enableDebugLog)
            {
                Debug.LogWarning("LevelManager: No VictoryManager found. Transitioning directly to outro scene.");
            }
            StartCoroutine(TransitionToOutroScene());
        }
    }
    
    void DestroyAllSpawnersImmediately()
    {
        foreach (var spawner in allSpawners)
        {
            if (spawner != null)
            {
                if (enableDebugLog)
                {
                    Debug.Log($"LevelManager: Destroying spawner {spawner.name}");
                }
                Destroy(spawner.gameObject);
            }
        }
        
        if (enableDebugLog)
        {
            Debug.Log("LevelManager: All spawners destroyed. Level complete!");
        }
    }
    
    System.Collections.IEnumerator DestroyAllSpawnersAfterDelay()
    {
        yield return new WaitForSeconds(completionDelay);
        
        foreach (var spawner in allSpawners)
        {
            if (spawner != null)
            {
                if (enableDebugLog)
                {
                    Debug.Log($"LevelManager: Destroying spawner {spawner.name}");
                }
                Destroy(spawner.gameObject);
            }
        }
        
        if (enableDebugLog)
        {
            Debug.Log("LevelManager: All spawners destroyed. Level complete!");
        }
    }
    
    System.Collections.IEnumerator TransitionToOutroScene()
    {
        // Wait for completion delay before transitioning
        yield return new WaitForSeconds(completionDelay);
        
        if (enableDebugLog)
        {
            Debug.Log($"LevelManager: Transitioning to outro scene: {outroSceneName}");
        }
        
        // Load the outro scene
        SceneManager.LoadScene(outroSceneName);
    }
    
    // Public methods for spawners to check global limits
    public bool CanSpawnVariant(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        if (limit == null) return false;
        
        // Check global capacity (max alive across all spawners)
        if (limit.hasReachedGlobalCapacity)
        {
            if (enableDebugLog)
            {
                Debug.Log($"LevelManager: Global capacity reached for {variantName}. Cannot spawn.");
            }
            return false;
        }
        
        // Check global maximum (total to spawn across all spawners)
        if (limit.hasReachedGlobalMaximum)
        {
            if (enableDebugLog)
            {
                Debug.Log($"LevelManager: Global maximum reached for {variantName}. Cannot spawn.");
            }
            return false;
        }
        
        return true;
    }
    
    public void OnEnemySpawned(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        if (limit != null)
        {
            limit.totalGlobalSpawnedCount++;
            limit.currentGlobalAliveCount++;
            
            // Check if global maximum is reached
            if (limit.totalGlobalSpawnedCount >= limit.globalMaximum)
            {
                limit.hasReachedGlobalMaximum = true;
                
                if (enableDebugLog)
                {
                    Debug.Log($"LevelManager: Global maximum reached for {variantName} ({limit.globalMaximum}). Stopping all spawning of this variant.");
                }
            }
            
            // Check if global capacity is reached
            if (limit.currentGlobalAliveCount >= limit.globalCapacity)
            {
                limit.hasReachedGlobalCapacity = true;
                
                if (enableDebugLog)
                {
                    Debug.Log($"LevelManager: Global capacity reached for {variantName} ({limit.globalCapacity}). Pausing all spawning of this variant.");
                }
            }
            
            if (enableDebugLog)
            {
                Debug.Log($"LevelManager: {variantName} spawned. Global: Alive {limit.currentGlobalAliveCount}/{limit.globalCapacity}, Total {limit.totalGlobalSpawnedCount}/{limit.globalMaximum}");
            }
        }
    }
    
    public void OnEnemyDied(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        if (limit != null)
        {
            limit.currentGlobalAliveCount = Mathf.Max(0, limit.currentGlobalAliveCount - 1);
            
            // Check if global capacity is freed
            if (limit.hasReachedGlobalCapacity && limit.currentGlobalAliveCount < limit.globalCapacity)
            {
                limit.hasReachedGlobalCapacity = false;
                
                if (enableDebugLog)
                {
                    Debug.Log($"LevelManager: Global capacity freed for {variantName}. Spawning can resume.");
                }
            }
            
            if (enableDebugLog)
            {
                Debug.Log($"LevelManager: {variantName} died. Global: Alive {limit.currentGlobalAliveCount}/{limit.globalCapacity}, Total {limit.totalGlobalSpawnedCount}/{limit.globalMaximum}");
            }
        }
    }
    
    // Public getters for external systems
    public int GetGlobalAliveCount(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.currentGlobalAliveCount : 0;
    }
    
    public int GetGlobalCapacity(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.globalCapacity : 0;
    }
    
    public int GetGlobalTotalSpawned(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.totalGlobalSpawnedCount : 0;
    }
    
    public int GetGlobalMaximum(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.globalMaximum : 0;
    }
    
    public bool IsGlobalCapacityReached(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.hasReachedGlobalCapacity : false;
    }
    
    public bool IsGlobalMaximumReached(string variantName)
    {
        var limit = globalLimits.Find(l => l.variantName == variantName);
        return limit != null ? limit.hasReachedGlobalMaximum : false;
    }
    
    public bool IsLevelComplete()
    {
        return levelComplete;
    }
    
    // Method to add global limits at runtime
    public void AddGlobalLimit(string variantName, GameObject prefab, int capacity, int maximum)
    {
        var existingLimit = globalLimits.Find(l => l.variantName == variantName);
        if (existingLimit == null)
        {
            var newLimit = new GlobalEnemyLimit
            {
                variantName = variantName,
                enemyPrefab = prefab,
                globalCapacity = capacity,
                globalMaximum = maximum,
                currentGlobalAliveCount = 0,
                totalGlobalSpawnedCount = 0,
                hasReachedGlobalCapacity = false,
                hasReachedGlobalMaximum = false
            };
            
            globalLimits.Add(newLimit);
            
            if (enableDebugLog)
            {
                Debug.Log($"LevelManager: Added global limit for {variantName} (Capacity: {capacity}, Maximum: {maximum})");
            }
        }
    }
    
    void OnGUI()
    {
        if (showGlobalCounts && enableDebugLog)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("Global Enemy Counts:", GUI.skin.box);
            
            foreach (var limit in globalLimits)
            {
                string status = "";
                if (limit.hasReachedGlobalMaximum) status = " [MAX]";
                else if (limit.hasReachedGlobalCapacity) status = " [CAP]";
                
                GUILayout.Label($"{limit.variantName}: {limit.currentGlobalAliveCount}/{limit.globalCapacity} alive, {limit.totalGlobalSpawnedCount}/{limit.globalMaximum} total{status}");
            }
            
            GUILayout.EndArea();
        }
    }
    
    void OnValidate()
    {
        // Ensure positive values
        foreach (var limit in globalLimits)
        {
            limit.globalCapacity = Mathf.Max(1, limit.globalCapacity);
            limit.globalMaximum = Mathf.Max(1, limit.globalMaximum);
            
            // Ensure capacity doesn't exceed maximum
            if (limit.globalCapacity > limit.globalMaximum)
            {
                limit.globalCapacity = limit.globalMaximum;
            }
        }
    }
}
