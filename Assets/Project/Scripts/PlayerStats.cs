using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    public TextMeshProUGUI healthText;

    // TODO: when health reaches zero player loses (GAME OVER)
    // FIXME: health shouldn't exceed maxHealth
    public void AddHealth(int amount)
    {
        health += amount;
        UpdateUI();
    }

    //TODO: Add damaging Itmes
    public void TakeDamage(int amount)
    {
        health -= amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + health;
        }
    }
}