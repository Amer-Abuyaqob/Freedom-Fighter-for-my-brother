using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI killCountText;
    [SerializeField] TextMeshProUGUI remainingEnemiesText;

    [Header("Settings")]
    [SerializeField] string healthFormat = "Health: {0}/{1}";
    [SerializeField] string killCountFormat = "Killed: {0}";
    [SerializeField] string remainingEnemiesFormat = "Remaining: {0}";

    [Header("Display Options")]
    [SerializeField] bool showRemainingEnemies = true;
    [SerializeField] bool showIndividualVariantCounts = false;

    PlayerStats playerStats;
    int totalEnemiesKilled = 0;
    LevelManager levelManager;

    public static HUDManager Instance { get; private set; }

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
        // Find player stats
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }

        // Find LevelManager
        levelManager = LevelManager.Instance;
        if (levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
        }

        // Update initial display
        UpdateHealthDisplay();
        UpdateKillCountDisplay();
        UpdateRemainingEnemiesDisplay();
    }

    void Update()
    {
        // Update health every frame (simple approach)
        UpdateHealthDisplay();
        
        // Update remaining enemies display
        if (showRemainingEnemies)
        {
            UpdateRemainingEnemiesDisplay();
        }
    }

    public void OnEnemyKilled()
    {
        totalEnemiesKilled++;
        UpdateKillCountDisplay();
    }

    public int GetEnemiesKilled() => totalEnemiesKilled;
    
    public int GetTotalEnemiesKilled() => totalEnemiesKilled;

    void UpdateHealthDisplay()
    {
        if (healthText == null || playerStats == null) return;
        healthText.text = string.Format(healthFormat, playerStats.health, playerStats.maxHealth);
    }

    void UpdateKillCountDisplay()
    {
        if (killCountText == null) return;
        killCountText.text = string.Format(killCountFormat, totalEnemiesKilled);
    }
    
    void UpdateRemainingEnemiesDisplay()
    {
        if (remainingEnemiesText == null || levelManager == null) return;
        
        int totalRemaining = CalculateTotalRemainingEnemies();
        
        if (showIndividualVariantCounts)
        {
            string detailedText = GetDetailedRemainingText();
            remainingEnemiesText.text = detailedText;
        }
        else
        {
            remainingEnemiesText.text = string.Format(remainingEnemiesFormat, totalRemaining);
        }
    }
    
    int CalculateTotalRemainingEnemies()
    {
        if (levelManager == null) return 0;
        
        int totalRemaining = 0;
        
        // Get all global limits from LevelManager
        var globalLimitsField = levelManager.GetType().GetField("globalLimits", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (globalLimitsField != null)
        {
            var globalLimits = globalLimitsField.GetValue(levelManager) as System.Collections.Generic.List<GlobalEnemyLimit>;
            
            if (globalLimits != null)
            {
                foreach (var limit in globalLimits)
                {
                    // Remaining = Global Maximum - Total Spawned + Currently Alive
                    int remaining = limit.globalMaximum - limit.totalGlobalSpawnedCount + limit.currentGlobalAliveCount;
                    totalRemaining += Mathf.Max(0, remaining);
                }
            }
        }
        
        return totalRemaining;
    }
    
    string GetDetailedRemainingText()
    {
        if (levelManager == null) return "Remaining: 0";
        
        string detailedText = "Remaining:\n";
        
        // Get all global limits from LevelManager
        var globalLimitsField = levelManager.GetType().GetField("globalLimits", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (globalLimitsField != null)
        {
            var globalLimits = globalLimitsField.GetValue(levelManager) as System.Collections.Generic.List<GlobalEnemyLimit>;
            
            if (globalLimits != null)
            {
                foreach (var limit in globalLimits)
                {
                    int remaining = limit.globalMaximum - limit.totalGlobalSpawnedCount + limit.currentGlobalAliveCount;
                    remaining = Mathf.Max(0, remaining);
                    
                    detailedText += $"{limit.variantName}: {remaining}\n";
                }
            }
        }
        
        return detailedText.TrimEnd('\n');
    }

    // Public methods for external updates
    public void SetHealthText(TextMeshProUGUI text) => healthText = text;
    public void SetKillCountText(TextMeshProUGUI text) => killCountText = text;
    public void SetRemainingEnemiesText(TextMeshProUGUI text) => remainingEnemiesText = text;
    
    // Public methods for display control
    public void SetShowRemainingEnemies(bool show)
    {
        showRemainingEnemies = show;
        if (remainingEnemiesText != null)
        {
            remainingEnemiesText.gameObject.SetActive(show);
        }
    }
    
    public void SetShowIndividualVariantCounts(bool show)
    {
        showIndividualVariantCounts = show;
        UpdateRemainingEnemiesDisplay();
    }
    
    // Public getters for external systems
    public int GetTotalRemainingEnemies()
    {
        return CalculateTotalRemainingEnemies();
    }
    
    public int GetRemainingEnemiesForVariant(string variantName)
    {
        if (levelManager == null) return 0;
        
        // Get all global limits from LevelManager
        var globalLimitsField = levelManager.GetType().GetField("globalLimits", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (globalLimitsField != null)
        {
            var globalLimits = globalLimitsField.GetValue(levelManager) as System.Collections.Generic.List<GlobalEnemyLimit>;
            
            if (globalLimits != null)
            {
                var limit = globalLimits.Find(l => l.variantName == variantName);
                if (limit != null)
                {
                    int remaining = limit.globalMaximum - limit.totalGlobalSpawnedCount + limit.currentGlobalAliveCount;
                    return Mathf.Max(0, remaining);
                }
            }
        }
        
        return 0;
    }
}
