using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour, IDamageable
{
    public int health = 100;
    public int maxHealth = 100;

    void Awake()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    // TODO: when health reaches zero player loses (GAME OVER)
    public void AddHealth(int amount)
    {
        if (amount <= 0) return;
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        health = Mathf.Max(0, health - amount);

        if (health == 0)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        // Load the GameOver scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}
