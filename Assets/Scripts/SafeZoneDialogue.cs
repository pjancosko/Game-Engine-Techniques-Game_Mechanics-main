using UnityEngine;

public class SafeZoneDialogue : MonoBehaviour
{
    public string[] safeZoneDialogue; // Set these lines in the Inspector
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger dialogue for the safe zone
            dialogueManager.StartDialogue(safeZoneDialogue);
        }
    }
}
