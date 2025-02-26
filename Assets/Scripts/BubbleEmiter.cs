using UnityEngine;

public class BubbleEmitter : MonoBehaviour
{
    public ParticleSystem bubbleParticles;

    private void Start()
    {
        if (bubbleParticles == null)
        {
            Debug.LogError("Bubble Particle System is not assigned in the Inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.gameObject.name);

        if (other.gameObject.name == "Ellen")
        {
            Debug.Log("Ellen entered the trigger.");

            if (bubbleParticles != null)
            {
                if (!bubbleParticles.isPlaying)
                {
                    bubbleParticles.Play();
                    Debug.Log("Bubble particles played.");
                }
                else
                {
                    Debug.Log("Bubble particles were already playing.");
                }
            }
            else
            {
                Debug.LogWarning("Bubble Particle System is not assigned.");
            }
        }
        else
        {
            Debug.Log("Other object entered the trigger: " + other.gameObject.name);
        }
    }
}