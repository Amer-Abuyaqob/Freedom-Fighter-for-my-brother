using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] UnityEngine.UI.Button playAgainButton;
    [SerializeField] UnityEngine.UI.Button mainMenuButton;
    [SerializeField] UnityEngine.UI.Button quitButton;

    [Header("Settings")]
    [SerializeField] string gameOverMessage = "GAME OVER";
    [SerializeField] string scoreFormat = "Enemies Defeated: {0}";

    void Start()
    {
        // Setup button listeners
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Update UI text
        UpdateUI();
    }

    void UpdateUI()
    {
        // Set game over text
        if (gameOverText != null)
            gameOverText.text = gameOverMessage;

        // Get score from HUDManager if available
        int enemiesKilled = 0;
        if (HUDManager.Instance != null)
        {
            // We'll need to add a public getter to HUDManager for enemies killed
            // For now, we'll use a simple approach
            enemiesKilled = GetEnemiesKilled();
        }

        // Set score text
        if (scoreText != null)
            scoreText.text = string.Format(scoreFormat, enemiesKilled);
    }

    int GetEnemiesKilled()
    {
        // Get score from HUDManager if available
        if (HUDManager.Instance != null)
        {
            return HUDManager.Instance.GetEnemiesKilled();
        }
        
        // Fallback: count remaining enemies (not accurate, but works as fallback)
        var enemies = FindObjectsOfType<EnemyStats>();
        return Mathf.Max(0, enemies.Length);
    }
    
    int GetRemainingEnemies()
    {
        // Get remaining enemies from HUDManager if available
        if (HUDManager.Instance != null)
        {
            return HUDManager.Instance.GetTotalRemainingEnemies();
        }
        
        return 0;
    }

    public void PlayAgain()
    {
        // Reload the current level (Level1)
        SceneManager.LoadScene("Level1");
    }

    public void GoToMainMenu()
    {
        // Go back to main menu
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // Quit the application
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Public method to set the score (called by other scripts)
    public void SetScore(int enemiesKilled)
    {
        if (scoreText != null)
            scoreText.text = string.Format(scoreFormat, enemiesKilled);
    }
}
