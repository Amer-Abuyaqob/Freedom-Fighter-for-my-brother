using System.Collections;
using UnityEngine;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] DialogueManager dialogueManager;
    
    [Header("Intro Settings")]
    [SerializeField] string introSequenceName = "IntroSequence";
    [SerializeField] string nextSceneName = "Level1";
    
    [Header("Background")]
    [SerializeField] GameObject backgroundImage;
    
    void Start()
    {
        // Find DialogueManager if not assigned
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
        
        // Set next scene
        if (dialogueManager != null)
        {
            dialogueManager.SetNextScene(nextSceneName);
        }
        
        // Start intro sequence
        StartCoroutine(StartIntroSequence());
    }
    
    IEnumerator StartIntroSequence()
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
            dialogueManager.StartDialogueSequence(introSequenceName);
        }
        else
        {
            Debug.LogError("IntroCutsceneController: DialogueManager not found!");
        }
    }
    
    // Public method to manually start intro (for testing)
    public void StartIntro()
    {
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogueSequence(introSequenceName);
        }
    }
}

