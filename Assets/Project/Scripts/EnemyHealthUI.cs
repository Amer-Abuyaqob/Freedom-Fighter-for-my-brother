using UnityEngine;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshPro healthText;
    [SerializeField] EnemyStats enemyStats;

    [Header("Display Settings")]
    [SerializeField] bool showMaxHealth = true;
    [SerializeField] string format = "{0}/{1}"; // {0} = current, {1} = max
    [SerializeField] bool hideWhenFull = false;

    void Awake()
    {
        if (healthText == null) healthText = GetComponent<TextMeshPro>();
        if (enemyStats == null) enemyStats = GetComponentInParent<EnemyStats>();
    }

    void Start()
    {
        if (enemyStats != null)
        {
            UpdateHealthDisplay();
        }
    }

    void Update()
    {
        // Update every frame (simple approach)
        // Later we can optimize with events if needed
        if (enemyStats != null)
        {
            UpdateHealthDisplay();
        }
    }

    void UpdateHealthDisplay()
    {
        if (healthText == null || enemyStats == null) return;

        int current = enemyStats.CurrentHealth;
        int max = enemyStats.MaxHealth;

        // Hide if full health and option is enabled
        if (hideWhenFull && current == max)
        {
            healthText.gameObject.SetActive(false);
            return;
        }

        healthText.gameObject.SetActive(true);

        if (showMaxHealth)
        {
            healthText.text = string.Format(format, current, max);
        }
        else
        {
            healthText.text = current.ToString();
        }
    }
}
