using System.Collections;
using UnityEngine;

public class OutroCutsceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] DialogueManager dialogueManager;
    
    [Header("Outro Settings")]
    [SerializeField] string outroSequenceName = "OutroSequence";
    [SerializeField] bool endGameAfterOutro = false; // End the game instead of going to another scene
    [SerializeField] string nextSceneName = "TBC"; // Scene to load if not ending game
    
    [Header("Background")]
    [SerializeField] GameObject backgroundImage;
    
    void Start()
    {
        // Find DialogueManager if not assigned
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
        
        // Set next scene or end game
        if (dialogueManager != null)
        {
            if (endGameAfterOutro)
            {
                dialogueManager.SetNextScene(""); // Empty means end game
            }
            else
            {
                dialogueManager.SetNextScene(nextSceneName);
            }
        }
        
        // Start outro sequence
        StartCoroutine(StartOutroSequence());
    }
    
    IEnumerator StartOutroSequence()
    {
        // Wait a frame to ensure everything is initialized
        yield return null;
        
        // Show background if available
        if (backgroundImage != null)
        {
            backgroundImage.SetActive(true);
        }
        
        // Wait a moment before starting dialogue
        yield return new WaitForSeconds(1f);
        
        // Start the dialogue sequence
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogueSequence(outroSequenceName);
        }
        else
        {
            Debug.LogError("OutroCutsceneController: DialogueManager not found!");
        }
    }
    
    // Public method to manually start outro (for testing)
    public void StartOutro()
    {
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogueSequence(outroSequenceName);
        }
    }
}

