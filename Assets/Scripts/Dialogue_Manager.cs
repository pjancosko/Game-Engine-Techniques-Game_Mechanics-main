using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Reference to your TextMeshProUGUI component
    public float typingSpeed = 0.05f;      // Time delay between each letter
    public float autoAdvanceDelay = 5f;    // Delay before auto-advancing dialogue

    private Queue<string> dialogueLines; // Holds the dialogue lines
    private bool isTyping = false;         // Indicates if the text is currently being typed out
    private Coroutine autoAdvanceCoroutine; // Reference to the auto-advance coroutine

    void Awake()
    {
        dialogueLines = new Queue<string>();
    }

    // Starts the dialogue sequence with an array of lines
    public void StartDialogue(string[] lines)
    {
        dialogueLines.Clear();
        foreach (string line in lines)
        {
            dialogueLines.Enqueue(line);
        }
        DisplayNextLine();
    }

    // Displays the next dialogue line
    public void DisplayNextLine()
    {
        // Stop any running auto-advance coroutine
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Get the next line and start typing it
        string line = dialogueLines.Dequeue();
        StopAllCoroutines(); // Stops any previous typing effects
        StartCoroutine(TypeSentence(line));
    }

    // Coroutine to type out the sentence letter by letter
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        // Start auto-advance after the line is fully displayed
        autoAdvanceCoroutine = StartCoroutine(AutoAdvance());
    }

    // Coroutine to wait for autoAdvanceDelay seconds before advancing
    IEnumerator AutoAdvance()
    {
        yield return new WaitForSeconds(autoAdvanceDelay);
        DisplayNextLine();
    }

    // Called when all dialogue lines have been shown
    void EndDialogue()
    {
        dialogueText.text = "";
        // Additional cleanup or events can be added here
    }

    // Listen for input to manually advance dialogue (e.g., spacebar)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            // If the player presses Space, cancel the auto-advance and show the next line immediately
            if (autoAdvanceCoroutine != null)
            {
                StopCoroutine(autoAdvanceCoroutine);
                autoAdvanceCoroutine = null;
            }
            DisplayNextLine();
        }
    }
}
