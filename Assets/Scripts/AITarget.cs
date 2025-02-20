using UnityEngine;
using UnityEngine.AI;

public class AITarget : MonoBehaviour
{
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;
    public Transform TargetPosition; // Assuming TargetPosition is a Transform
    public float AttackDistance = 2.0f; // Example attack distance

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Example usage of m_Distance
        m_Distance = Vector3.Distance(m_Agent.transform.position, TargetPosition.position);
        if (m_Distance < AttackDistance)
        {
            m_Agent.isStopped = true;
            m_Animator.SetBool("Attack", true);
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.destination = TargetPosition.position;
            m_Animator.SetBool("Attack", false);
        }
    }

    void OnAnimatorMove()
    {
        if (m_Animator.GetBool("Attack") == false)
        {
            Vector3 deltaPosition = m_Animator.deltaPosition;
            if (deltaPosition != Vector3.zero)
            {
                m_Agent.speed = deltaPosition.magnitude / Time.deltaTime;
            }
        }
    }

    // Function to be called by Animation Event
    public void PlayStep()
    {
        Debug.Log("Step sound played");
        // Add step sound effect or footstep logic here
    }

    // Function to be called by Animation Event 'StartAttack'
    public void StartAttack()
    {
        Debug.Log("Grenadier started attack animation");
        // Add attack logic here (e.g., apply damage, spawn hit effects, etc.)
    }

    // Function to be called by Animation Event 'EndAttack'
    public void EndAttack()
    {
        Debug.Log("Grenadier ended attack animation");
        // Add logic here to reset attack state, cooldowns, etc.
    }
}
