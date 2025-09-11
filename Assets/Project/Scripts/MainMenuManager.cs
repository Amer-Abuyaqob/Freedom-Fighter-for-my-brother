using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] UnityEngine.UI.Button startButton;
    [SerializeField] UnityEngine.UI.Button quitButton;

    [Header("Settings")]
    [SerializeField] string gameTitle = "Freedom Fighter: For My Brother";
    [SerializeField] string startButtonText = "START GAME";
    [SerializeField] string quitButtonText = "QUIT";

    void Start()
    {
        // Setup button listeners
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Update UI text
        UpdateUI();
    }

    void UpdateUI()
    {
        // Set title text
        if (titleText != null)
            titleText.text = gameTitle;

        // Set button text if they have TextMeshProUGUI components
        if (startButton != null)
        {
            var startText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            if (startText != null)
                startText.text = startButtonText;
        }

        if (quitButton != null)
        {
            var quitText = quitButton.GetComponentInChildren<TextMeshProUGUI>();
            if (quitText != null)
                quitText.text = quitButtonText;
        }
    }

    public void StartGame()
    {
        // Load Intro_Lv1 scene
        SceneManager.LoadScene("Intro_Lv1");
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
    public void SetGameTitle(string newTitle)
    {
        gameTitle = newTitle;
        if (titleText != null)
            titleText.text = gameTitle;
    }

    public void SetStartButtonText(string newText)
    {
        startButtonText = newText;
        if (startButton != null)
        {
            var startText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            if (startText != null)
                startText.text = startButtonText;
        }
    }

    public void SetQuitButtonText(string newText)
    {
        quitButtonText = newText;
        if (quitButton != null)
        {
            var quitText = quitButton.GetComponentInChildren<TextMeshProUGUI>();
            if (quitText != null)
                quitText.text = quitButtonText;
        }
    }
}
