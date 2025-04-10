using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DialogueManager dialogueManager; // Reference to your Dialogue Manager
    public string[] introDialogue; // Set these lines in the Inspector

    void Start()
    {
        // Start the game with an introduction dialogue
        dialogueManager.StartDialogue(introDialogue);
    }
}

