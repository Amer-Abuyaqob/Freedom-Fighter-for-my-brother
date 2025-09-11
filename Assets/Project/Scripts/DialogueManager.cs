using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine
{
    [Header("Dialogue Content")]
    [TextArea(3, 5)]
    public string text;
    
    [Header("Speaker Info")]
    public string speakerName = "";
    public Sprite speakerPortrait;
    
    [Header("Timing")]
    public float displayDuration = 3f; // Auto-advance after this time
    public bool waitForInput = true; // Wait for Next button click
}

[System.Serializable]
public class DialogueSequence
{
    [Header("Sequence Info")]
    public string sequenceName;
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
    
    [Header("Settings")]
    public bool autoAdvance = false; // Auto-advance through all lines
    public float autoAdvanceDelay = 0.5f; // Delay between auto-advances
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI speakerNameText;
    [SerializeField] Image speakerPortraitImage;
    [SerializeField] Button nextButton;
    [SerializeField] Button skipButton;
    
    [Header("Dialogue Sequences")]
    [SerializeField] List<DialogueSequence> dialogueSequences = new List<DialogueSequence>();
    
    [Header("Settings")]
    [SerializeField] float textSpeed = 0.05f; // Speed of typewriter effect
    [SerializeField] bool enableTypewriterEffect = true;
    [SerializeField] string nextButtonText = "Next";
    [SerializeField] string skipButtonText = "Skip";
    
    [Header("Scene Transitions")]
    [SerializeField] string nextSceneName = ""; // Scene to load after dialogue ends
    
    // Current state
    DialogueSequence currentSequence;
    int currentLineIndex = 0;
    bool isDisplayingText = false;
    bool isSequenceActive = false;
    Coroutine typewriterCoroutine;
    
    public static DialogueManager Instance { get; private set; }
    
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
        SetupUI();
        SetupButtons();
        
        // Hide dialogue panel initially
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
    
    void SetupUI()
    {
        // Setup button text
        if (nextButton != null)
        {
            var nextText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
            if (nextText != null)
                nextText.text = nextButtonText;
        }
        
        if (skipButton != null)
        {
            var skipText = skipButton.GetComponentInChildren<TextMeshProUGUI>();
            if (skipText != null)
                skipText.text = skipButtonText;
        }
    }
    
    void SetupButtons()
    {
        // Setup button listeners
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButtonClicked);
        
        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipButtonClicked);
    }
    
    // Public method to start a dialogue sequence
    public void StartDialogueSequence(string sequenceName)
    {
        var sequence = dialogueSequences.Find(s => s.sequenceName == sequenceName);
        if (sequence == null)
        {
            Debug.LogError($"DialogueManager: Sequence '{sequenceName}' not found!");
            return;
        }
        
        StartDialogueSequence(sequence);
    }
    
    public void StartDialogueSequence(DialogueSequence sequence)
    {
        if (sequence == null || sequence.dialogueLines.Count == 0)
        {
            Debug.LogError("DialogueManager: Invalid sequence provided!");
            return;
        }
        
        currentSequence = sequence;
        currentLineIndex = 0;
        isSequenceActive = true;
        
        // Show dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        
        // Display first line
        DisplayCurrentLine();
    }
    
    void DisplayCurrentLine()
    {
        if (currentSequence == null || currentLineIndex >= currentSequence.dialogueLines.Count)
        {
            EndDialogueSequence();
            return;
        }
        
        var currentLine = currentSequence.dialogueLines[currentLineIndex];
        
        // Update speaker info
        UpdateSpeakerInfo(currentLine);
        
        // Display text
        if (enableTypewriterEffect)
        {
            StartTypewriterEffect(currentLine.text);
        }
        else
        {
            dialogueText.text = currentLine.text;
            isDisplayingText = false;
        }
        
        // Setup auto-advance if enabled
        if (currentSequence.autoAdvance && !currentLine.waitForInput)
        {
            StartCoroutine(AutoAdvanceCoroutine(currentLine.displayDuration));
        }
    }
    
    void UpdateSpeakerInfo(DialogueLine line)
    {
        // Update speaker name
        if (speakerNameText != null)
        {
            speakerNameText.text = line.speakerName;
            speakerNameText.gameObject.SetActive(!string.IsNullOrEmpty(line.speakerName));
        }
        
        // Update speaker portrait
        if (speakerPortraitImage != null)
        {
            if (line.speakerPortrait != null)
            {
                speakerPortraitImage.sprite = line.speakerPortrait;
                speakerPortraitImage.gameObject.SetActive(true);
            }
            else
            {
                speakerPortraitImage.gameObject.SetActive(false);
            }
        }
    }
    
    void StartTypewriterEffect(string text)
    {
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        
        typewriterCoroutine = StartCoroutine(TypewriterCoroutine(text));
    }
    
    IEnumerator TypewriterCoroutine(string text)
    {
        isDisplayingText = true;
        dialogueText.text = "";
        
        for (int i = 0; i <= text.Length; i++)
        {
            dialogueText.text = text.Substring(0, i);
            yield return new WaitForSeconds(textSpeed);
        }
        
        isDisplayingText = false;
    }
    
    IEnumerator AutoAdvanceCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isSequenceActive)
        {
            AdvanceToNextLine();
        }
    }
    
    void OnNextButtonClicked()
    {
        if (!isSequenceActive) return;
        
        if (isDisplayingText)
        {
            // Skip typewriter effect
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            
            var currentLine = currentSequence.dialogueLines[currentLineIndex];
            dialogueText.text = currentLine.text;
            isDisplayingText = false;
        }
        else
        {
            AdvanceToNextLine();
        }
    }
    
    void OnSkipButtonClicked()
    {
        EndDialogueSequence();
    }
    
    void AdvanceToNextLine()
    {
        currentLineIndex++;
        DisplayCurrentLine();
    }
    
    void EndDialogueSequence()
    {
        isSequenceActive = false;
        currentSequence = null;
        currentLineIndex = 0;
        
        // Hide dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
        // Load next scene if specified, otherwise end the game
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // End the game
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    
    // Public methods for external control
    public bool IsDialogueActive()
    {
        return isSequenceActive;
    }
    
    public void SetNextScene(string sceneName)
    {
        nextSceneName = sceneName;
    }
    
    // Method to add sequences at runtime
    public void AddDialogueSequence(DialogueSequence sequence)
    {
        if (sequence != null)
        {
            dialogueSequences.Add(sequence);
        }
    }
    
    void Update()
    {
        // Allow Enter/Space to advance dialogue
        if (isSequenceActive && !isDisplayingText)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                OnNextButtonClicked();
            }
        }
    }
}

