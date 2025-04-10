using UnityEngine;

public class HeartDialogueTrigger : MonoBehaviour
{
    public string[] heartDialogue; // Set the dialogue lines in the Inspector
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(heartDialogue);
        }
    }
}
