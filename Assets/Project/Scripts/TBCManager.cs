using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TBCManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI youWonText;
    [SerializeField] TextMeshProUGUI tbcText;
    [SerializeField] UnityEngine.UI.Button playAgainButton;
    [SerializeField] UnityEngine.UI.Button mainMenuButton;
    [SerializeField] UnityEngine.UI.Button quitButton;

    [Header("Settings")]
    [SerializeField] string youWonMessage = "You Won";
    [SerializeField] string tbcMessage = "To Be Continued";

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
        // Set "You Won" text
        if (youWonText != null)
            youWonText.text = youWonMessage;

        // Set "To Be Continued" text
        if (tbcText != null)
            tbcText.text = tbcMessage;
    }

    public void PlayAgain()
    {
        // Start from the beginning (Intro_Lv1)
        SceneManager.LoadScene("Intro_Lv1");
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

    // Public methods to update text at runtime
    public void SetYouWonText(string text)
    {
        youWonMessage = text;
        if (youWonText != null)
            youWonText.text = youWonMessage;
    }

    public void SetTBCText(string text)
    {
        tbcMessage = text;
        if (tbcText != null)
            tbcText.text = tbcMessage;
    }
}
