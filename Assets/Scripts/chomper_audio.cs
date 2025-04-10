using UnityEngine;

public class ChomperAudio : MonoBehaviour
{
    // Called by the PlayStep AnimationEvent on "ChomperIdle"
    public void PlayStep()
    {
        Debug.Log("Step played!");
        // Add step sound playback logic here, for example:
        // GetComponent<AudioSource>().Play();
    }

    // Called by the Grunt AnimationEvent on "ChomperIdle"
    public void Grunt()
    {
        Debug.Log("Grunt sound played!");
        // Add grunt sound playback logic here
    }

    // Called by the AttackBegin AnimationEvent on "ChomperAttack"
    public void AttackBegin()
    {
        Debug.Log("Attack began!");
        // Add logic to handle the beginning of an attack,
        // such as playing an attack sound or enabling hit detection
    }

    // Called by the AttackEnd AnimationEvent on "ChomperAttack"
    public void AttackEnd()
    {
        Debug.Log("Attack ended!");
        // Add logic for finishing the attack,
        // such as disabling hit detection or transitioning back to idle
    }
}
