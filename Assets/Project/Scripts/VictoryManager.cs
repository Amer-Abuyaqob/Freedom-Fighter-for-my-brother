using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject victoryPanel;
    [SerializeField] TextMeshProUGUI victoryText;
    [SerializeField] UnityEngine.UI.Button continueButton;
    
    [Header("Victory Settings")]
    [SerializeField] string victoryMessage = "You Won!";
    [SerializeField] float victoryDelay = 3f; // Delay before showing victory message
    [SerializeField] float continueDelay = 2f; // Delay before showing continue button
    [SerializeField] string nextSceneName = "Outro_Lv1";
    
    [Header("Animation Settings")]
    [SerializeField] bool animateText = true;
    [SerializeField] float textAnimationSpeed = 0.1f;
    [SerializeField] Color victoryTextColor;
    
    [Header("Debug")]
    [SerializeField] bool enableDebugLog = false;
    
    bool victoryTriggered = false;
    bool victorySequenceComplete = false;
    
    public static VictoryManager Instance { get; private set; }
    
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
        // Initialize UI
        InitializeUI();
        
        // Subscribe to level completion
        if (LevelManager.Instance != null)
        {
            // We'll check for level completion in Update since LevelManager doesn't have events
        }
    }
    
    void Update()
    {
        // Check if level is complete and victory hasn't been triggered yet
        if (!victoryTriggered && LevelManager.Instance != null && LevelManager.Instance.IsLevelComplete())
        {
            TriggerVictorySequence();
        }
    }
    
    void InitializeUI()
    {
        // Hide victory panel initially
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        
        // Setup continue button
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueToNextScene);
            continueButton.gameObject.SetActive(false);
        }
        
        // Setup victory text
        if (victoryText != null)
        {
            victoryText.text = "";
            victoryText.color = victoryTextColor;
        }
    }
    
    public void TriggerVictorySequence()
    {
        if (victoryTriggered) return;
        
        victoryTriggered = true;
        
        if (enableDebugLog)
        {
            Debug.Log("VictoryManager: Victory sequence triggered!");
        }
        
        // Start the victory sequence
        StartCoroutine(VictorySequenceCoroutine());
    }
    
    System.Collections.IEnumerator VictorySequenceCoroutine()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(victoryDelay);
        
        // Show victory panel
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        
        // Animate or display victory text
        if (victoryText != null)
        {
            if (animateText)
            {
                yield return StartCoroutine(AnimateText(victoryMessage));
            }
            else
            {
                victoryText.text = victoryMessage;
            }
        }
        
        // Wait before showing continue button
        yield return new WaitForSeconds(continueDelay);
        
        // Show continue button
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
        
        victorySequenceComplete = true;
        
        if (enableDebugLog)
        {
            Debug.Log("VictoryManager: Victory sequence complete!");
        }
    }
    
    System.Collections.IEnumerator AnimateText(string text)
    {
        victoryText.text = "";
        
        for (int i = 0; i < text.Length; i++)
        {
            victoryText.text += text[i];
            yield return new WaitForSeconds(textAnimationSpeed);
        }
    }
    
    public void ContinueToNextScene()
    {
        if (enableDebugLog)
        {
            Debug.Log($"VictoryManager: Continuing to {nextSceneName}");
        }
        
        SceneManager.LoadScene(nextSceneName);
    }
    
    // Public methods for external access
    public bool IsVictoryTriggered()
    {
        return victoryTriggered;
    }
    
    public bool IsVictorySequenceComplete()
    {
        return victorySequenceComplete;
    }
    
    public void SetVictoryMessage(string message)
    {
        victoryMessage = message;
    }
    
    public void SetNextScene(string sceneName)
    {
        nextSceneName = sceneName;
    }
    
    public void SetVictoryDelay(float delay)
    {
        victoryDelay = delay;
    }
    
    public void SetContinueDelay(float delay)
    {
        continueDelay = delay;
    }
    
    // Method to manually trigger victory (for testing)
    [ContextMenu("Test Victory Sequence")]
    public void TestVictorySequence()
    {
        if (!victoryTriggered)
        {
            TriggerVictorySequence();
        }
    }
}
