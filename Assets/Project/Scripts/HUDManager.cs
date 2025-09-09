using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI killCountText;

    [Header("Settings")]
    [SerializeField] string healthFormat = "Health: {0}/{1}";
    [SerializeField] string killCountFormat = "Killed: {0}/{1}";

    PlayerStats playerStats;
    int totalEnemies = 0;
    int enemiesKilled = 0;

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

        // Count total enemies in scene
        CountTotalEnemies();

        // Update initial display
        UpdateHealthDisplay();
        UpdateKillCountDisplay();
    }

    void Update()
    {
        // Update health every frame (simple approach)
        UpdateHealthDisplay();
    }

    void CountTotalEnemies()
    {
        var enemies = FindObjectsOfType<EnemyStats>();
        totalEnemies = enemies.Length;
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        UpdateKillCountDisplay();
    }

    void UpdateHealthDisplay()
    {
        if (healthText == null || playerStats == null) return;
        healthText.text = string.Format(healthFormat, playerStats.health, playerStats.maxHealth);
    }

    void UpdateKillCountDisplay()
    {
        if (killCountText == null) return;
        killCountText.text = string.Format(killCountFormat, enemiesKilled, totalEnemies);
    }

    // Public methods for external updates
    public void SetHealthText(TextMeshProUGUI text) => healthText = text;
    public void SetKillCountText(TextMeshProUGUI text) => killCountText = text;
}
