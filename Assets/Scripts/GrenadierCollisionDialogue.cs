using UnityEngine;

public class GrenadierCollisionDialogue : MonoBehaviour
{
    // Dialogue lines to be displayed when collision occurs
    public string[] collisionDialogue = new string[] {
        "Human detected! Destroy enabled.",
        "Ouch, I need to find a safe spot."
    };

    private DialogueManager dialogueManager;

    void Start()
    {
        // Find the DialogueManager in the scene
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found in the scene. Please add one.");
        }
    }

    // Trigger is used because Grenadier's collider is set as a trigger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);
        // Check if the colliding object is tagged as "Player" (Ellen)
        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(collisionDialogue);
        }
    }
}
