using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollectables : MonoBehaviour
{
    [SerializeField] Item[] itemPrefabs;
    [SerializeField] float spawnInterval = 5f;
    [SerializeField] float holdTime = 3f;
    [SerializeField] Vector2 xRange = new Vector2(24f, 36f);
    [SerializeField] Vector2 yRange = new Vector2(7.74f, 10.4f);
    
    [Header("Spacing Settings")]
    [SerializeField] float minDistanceFromOtherMedkits = 5f; // Minimum distance from other medkits
    [SerializeField] int maxSpawnAttempts = 10; // Maximum attempts to find a valid spawn position
    
    [Header("Capacity Management")]
    [SerializeField] bool useCapacityLimits = true;
    [SerializeField] string[] variantNames; // Must match HealerCapacityManager variant names

    void Start()
    {
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            float delay = 1f + i * holdTime;
            StartCoroutine(SpawnLoop(itemPrefabs[i], i, delay));
        }
    }

    private void Spawn(Item prefab, int prefabIndex)
    {
        if (prefab == null) return;

        // Check capacity limits if enabled
        if (useCapacityLimits && HealerCapacityManager.Instance != null)
        {
            string variantName = GetVariantName(prefabIndex);
            if (!HealerCapacityManager.Instance.CanSpawnHealer(variantName))
            {
                // Capacity reached, skip spawning
                return;
            }
        }

        // Try to find a valid spawn position
        Vector3 spawnPos = FindValidSpawnPosition();
        if (spawnPos == Vector3.zero)
        {
            // Could not find a valid position, skip spawning
            Debug.LogWarning($"SpawnCollectables: Could not find valid spawn position for {prefab.name} after {maxSpawnAttempts} attempts");
            return;
        }

        GameObject spawnedItem = Instantiate(prefab.gameObject, spawnPos, Quaternion.identity);
        
        // Notify capacity manager if enabled
        if (useCapacityLimits && HealerCapacityManager.Instance != null)
        {
            string variantName = GetVariantName(prefabIndex);
            HealerCapacityManager.Instance.OnHealerSpawned(variantName);
        }
    }

    private IEnumerator SpawnLoop(Item prefab, int prefabIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        while (true)
        {
            Spawn(prefab, prefabIndex);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private Vector3 FindValidSpawnPosition()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 candidatePos = new Vector3(
                Random.Range(xRange.x, xRange.y),
                Random.Range(yRange.x, yRange.y),
                0f
            );
            
            // Check if this position is far enough from other medkits
            if (IsPositionValid(candidatePos))
            {
                return candidatePos;
            }
        }
        
        // Return zero vector if no valid position found
        return Vector3.zero;
    }
    
    private bool IsPositionValid(Vector3 position)
    {
        // Find all existing medkits (Healer components)
        Healer[] existingMedkits = FindObjectsOfType<Healer>();
        
        foreach (Healer medkit in existingMedkits)
        {
            if (medkit != null)
            {
                float distance = Vector3.Distance(position, medkit.transform.position);
                if (distance < minDistanceFromOtherMedkits)
                {
                    return false; // Too close to an existing medkit
                }
            }
        }
        
        return true; // Position is valid
    }
    
    private string GetVariantName(int prefabIndex)
    {
        // Use variant name if provided, otherwise use prefab name
        if (variantNames != null && prefabIndex < variantNames.Length && !string.IsNullOrEmpty(variantNames[prefabIndex]))
        {
            return variantNames[prefabIndex];
        }
        
        // Fallback to prefab name
        if (itemPrefabs[prefabIndex] != null)
        {
            return itemPrefabs[prefabIndex].name;
        }
        
        return "UnknownVariant";
    }
}