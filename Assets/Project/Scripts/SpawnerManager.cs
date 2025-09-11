using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [Header("Spawner Management")]
    [SerializeField] List<EnemySpawner> spawners = new List<EnemySpawner>();
    [SerializeField] bool autoFindSpawners = true;
    [SerializeField] bool startAllSpawnersOnStart = true;
    
    [Header("Global Settings")]
    [SerializeField] float globalSpawnDelay = 1f;
    [SerializeField] bool enableDebugLog = false;
    
    [Header("Level Completion")]
    [SerializeField] bool checkLevelCompletion = true;
    [SerializeField] float completionCheckInterval = 2f;
    
    int totalSpawners = 0;
    int completedSpawners = 0;
    
    public static SpawnerManager Instance { get; private set; }
    
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
        if (autoFindSpawners)
        {
            FindAllSpawners();
        }
        
        if (startAllSpawnersOnStart)
        {
            StartAllSpawners();
        }
        
        if (checkLevelCompletion)
        {
            InvokeRepeating(nameof(CheckLevelCompletion), completionCheckInterval, completionCheckInterval);
        }
    }
    
    void FindAllSpawners()
    {
        spawners.Clear();
        var foundSpawners = FindObjectsOfType<EnemySpawner>();
        
        foreach (var spawner in foundSpawners)
        {
            AddSpawner(spawner);
        }
        
        totalSpawners = spawners.Count;
        
        if (enableDebugLog)
        {
            Debug.Log($"SpawnerManager: Found {totalSpawners} spawners in scene");
        }
    }
    
    public void AddSpawner(EnemySpawner spawner)
    {
        if (spawner != null && !spawners.Contains(spawner))
        {
            spawners.Add(spawner);
            totalSpawners = spawners.Count;
            
            if (enableDebugLog)
            {
                Debug.Log($"SpawnerManager: Added spawner {spawner.name}. Total: {totalSpawners}");
            }
        }
    }
    
    public void RemoveSpawner(EnemySpawner spawner)
    {
        if (spawners.Contains(spawner))
        {
            spawners.Remove(spawner);
            totalSpawners = spawners.Count;
            
            if (enableDebugLog)
            {
                Debug.Log($"SpawnerManager: Removed spawner {spawner.name}. Total: {totalSpawners}");
            }
        }
    }
    
    public void StartAllSpawners()
    {
        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.StartSpawning();
            }
        }
        
        if (enableDebugLog)
        {
            Debug.Log("SpawnerManager: Started all spawners");
        }
    }
    
    public void StopAllSpawners()
    {
        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.StopSpawning();
            }
        }
        
        if (enableDebugLog)
        {
            Debug.Log("SpawnerManager: Stopped all spawners");
        }
    }
    
    public void StartSpawner(EnemySpawner spawner)
    {
        if (spawner != null)
        {
            spawner.StartSpawning();
            
            if (enableDebugLog)
            {
                Debug.Log($"SpawnerManager: Started spawner {spawner.name}");
            }
        }
    }
    
    public void StopSpawner(EnemySpawner spawner)
    {
        if (spawner != null)
        {
            spawner.StopSpawning();
            
            if (enableDebugLog)
            {
                Debug.Log($"SpawnerManager: Stopped spawner {spawner.name}");
            }
        }
    }
    
    void CheckLevelCompletion()
    {
        completedSpawners = 0;
        
        foreach (var spawner in spawners)
        {
            if (spawner != null && spawner.IsAllVariantsReachedMax())
            {
                completedSpawners++;
            }
        }
        
        // Check if all spawners are completed
        if (completedSpawners >= totalSpawners && totalSpawners > 0)
        {
            OnAllSpawnersCompleted();
        }
    }
    
    void OnAllSpawnersCompleted()
    {
        if (enableDebugLog)
        {
            Debug.Log("SpawnerManager: All spawners completed! Level cleared!");
        }
        
        // Notify HUD or other systems that level is complete
        if (HUDManager.Instance != null)
        {
            // You could add a level completion method to HUDManager
            // HUDManager.Instance.OnLevelCompleted();
        }
        
        // Stop checking for completion
        CancelInvoke(nameof(CheckLevelCompletion));
    }
    
    // Public getters for external systems
    public int GetTotalSpawners() => totalSpawners;
    public int GetCompletedSpawners() => completedSpawners;
    public int GetActiveSpawners()
    {
        int active = 0;
        foreach (var spawner in spawners)
        {
            if (spawner != null && !spawner.IsAllVariantsReachedMax())
            {
                active++;
            }
        }
        return active;
    }
    
    public bool IsLevelComplete() => completedSpawners >= totalSpawners && totalSpawners > 0;
    
    public List<EnemySpawner> GetAllSpawners() => new List<EnemySpawner>(spawners);
    
    public EnemySpawner GetSpawnerByName(string name)
    {
        return spawners.Find(s => s.name == name);
    }
    
    // Method to get total enemy counts across all spawners
    public int GetTotalEnemyCount()
    {
        int total = 0;
        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                // This would require adding a method to EnemySpawner to get total count
                // total += spawner.GetTotalEnemyCount();
            }
        }
        return total;
    }
    
    void OnDestroy()
    {
        // Clean up
        spawners.Clear();
    }
}
