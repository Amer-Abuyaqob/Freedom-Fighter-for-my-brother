using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    [Header("Global Level Limits")]
    [SerializeField] GameObject settler1Prefab;
    [SerializeField] GameObject settler2Prefab;
    
    [Header("Settler1 Global Limits")]
    [SerializeField] int settler1GlobalCapacity = 8;
    [SerializeField] int settler1GlobalMaximum = 25;
    
    [Header("Settler2 Global Limits")]
    [SerializeField] int settler2GlobalCapacity = 12;
    [SerializeField] int settler2GlobalMaximum = 35;
    
    [Header("Level Settings")]
    [SerializeField] bool destroySpawnersWhenComplete = true;
    [SerializeField] float completionDelay = 2f;
    
    [Header("Auto Setup")]
    [SerializeField] bool setupOnStart = true;
    
    LevelManager levelManager;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupLevel();
        }
    }
    
    [ContextMenu("Setup Level")]
    public void SetupLevel()
    {
        // Get or create LevelManager
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            GameObject levelManagerObj = new GameObject("LevelManager");
            levelManager = levelManagerObj.AddComponent<LevelManager>();
        }
        
        // Add global limits
        if (settler1Prefab != null)
        {
            levelManager.AddGlobalLimit("Settler1", settler1Prefab, settler1GlobalCapacity, settler1GlobalMaximum);
        }
        
        if (settler2Prefab != null)
        {
            levelManager.AddGlobalLimit("Settler2", settler2Prefab, settler2GlobalCapacity, settler2GlobalMaximum);
        }
        
        Debug.Log($"LevelSetup: Configured global limits - Settler1 (Capacity: {settler1GlobalCapacity}, Maximum: {settler1GlobalMaximum}), Settler2 (Capacity: {settler2GlobalCapacity}, Maximum: {settler2GlobalMaximum})");
    }
    
    // Public methods for runtime configuration
    public void SetSettler1GlobalCapacity(int capacity)
    {
        settler1GlobalCapacity = capacity;
        if (levelManager != null)
        {
            // Update existing limit
            var limit = levelManager.GetType().GetField("globalLimits", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(levelManager) as System.Collections.Generic.List<GlobalEnemyLimit>;
            
            if (limit != null)
            {
                var settler1Limit = limit.Find(l => l.variantName == "Settler1");
                if (settler1Limit != null)
                {
                    settler1Limit.globalCapacity = capacity;
                }
            }
        }
    }
    
    public void SetSettler1GlobalMaximum(int maximum)
    {
        settler1GlobalMaximum = maximum;
        if (levelManager != null)
        {
            // Update existing limit
            var limit = levelManager.GetType().GetField("globalLimits", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(levelManager) as System.Collections.Generic.List<GlobalEnemyLimit>;
            
            if (limit != null)
            {
                var settler1Limit = limit.Find(l => l.variantName == "Settler1");
                if (settler1Limit != null)
                {
                    settler1Limit.globalMaximum = maximum;
                }
            }
        }
    }
    
    public void SetSettler2GlobalCapacity(int capacity)
    {
        settler2GlobalCapacity = capacity;
        if (levelManager != null)
        {
            // Update existing limit
            var limit = levelManager.GetType().GetField("globalLimits", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(levelManager) as System.Collections.Generic.List<GlobalEnemyLimit>;
            
            if (limit != null)
            {
                var settler2Limit = limit.Find(l => l.variantName == "Settler2");
                if (settler2Limit != null)
                {
                    settler2Limit.globalCapacity = capacity;
                }
            }
        }
    }
    
    public void SetSettler2GlobalMaximum(int maximum)
    {
        settler2GlobalMaximum = maximum;
        if (levelManager != null)
        {
            // Update existing limit
            var limit = levelManager.GetType().GetField("globalLimits", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(levelManager) as System.Collections.Generic.List<GlobalEnemyLimit>;
            
            if (limit != null)
            {
                var settler2Limit = limit.Find(l => l.variantName == "Settler2");
                if (settler2Limit != null)
                {
                    settler2Limit.globalMaximum = maximum;
                }
            }
        }
    }
    
    void OnValidate()
    {
        // Ensure positive values
        settler1GlobalCapacity = Mathf.Max(1, settler1GlobalCapacity);
        settler1GlobalMaximum = Mathf.Max(1, settler1GlobalMaximum);
        settler2GlobalCapacity = Mathf.Max(1, settler2GlobalCapacity);
        settler2GlobalMaximum = Mathf.Max(1, settler2GlobalMaximum);
        completionDelay = Mathf.Max(0f, completionDelay);
        
        // Ensure capacity doesn't exceed maximum
        if (settler1GlobalCapacity > settler1GlobalMaximum)
            settler1GlobalCapacity = settler1GlobalMaximum;
        if (settler2GlobalCapacity > settler2GlobalMaximum)
            settler2GlobalCapacity = settler2GlobalMaximum;
    }
}