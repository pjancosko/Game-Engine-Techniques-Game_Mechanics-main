using UnityEngine;

public class ChomperController : MonoBehaviour
{
    private Animator animator;

    // Cache the Animator component when the object starts.
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    // Called when another collider enters the trigger collider.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered collision with Chomper.");
            animator.SetBool("isAttacking", true);
        }
    }
    
    // Called when another collider exits the trigger collider.
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited collision with Chomper.");
            animator.SetBool("isAttacking", false);
        }
    }
}
