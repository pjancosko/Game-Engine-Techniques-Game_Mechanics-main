using UnityEngine;
using UnityEngine.AI;

public class ChomperAITarget : MonoBehaviour
{
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;
    public Transform TargetPosition; // Reference to the target's Transform (e.g., the player)
    public float AttackDistance = 2.0f; // Maximum distance for initiating attack
    private bool isInContact = false;  // Flag to indicate contact with the target

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        if (m_Agent == null)
        {
            Debug.LogError("No NavMeshAgent component found on Chomper!");
        }
        
        m_Animator = GetComponent<Animator>();
        if (m_Animator == null)
        {
            Debug.LogError("No Animator component found on Chomper!");
        }
    }

    void Update()
{
    if (m_Agent != null && m_Agent.isOnNavMesh)
    {
        m_Distance = Vector3.Distance(m_Agent.transform.position, TargetPosition.position);
        Debug.Log("Distance to target: " + m_Distance);

        if (m_Distance < AttackDistance && isInContact)
        {
            m_Agent.isStopped = true;
            m_Animator.SetBool("Attack", true);
            Debug.Log("Chomper is attacking!");
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.destination = TargetPosition.position;
            m_Animator.SetBool("Attack", false);
            Debug.Log("Chomper is chasing Ellen!");
        }
    }
}

    void OnAnimatorMove()
    {
        if (!m_Animator.GetBool("Attack"))
        {
            Vector3 deltaPosition = m_Animator.deltaPosition;
            if (deltaPosition != Vector3.zero)
            {
                m_Agent.speed = deltaPosition.magnitude / Time.deltaTime;
            }
        }
    }

    // Trigger detection assumes Chomper has a Collider set as Trigger.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInContact = true;
            Debug.Log("Player contact detected: Ready to attack!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInContact = false;
            Debug.Log("Player left attack range.");
        }
    }

    // Function called by Animation Event 'PlayStep'
    public void PlayStep()
    {
        Debug.Log("Chomper step sound played");
    }

    // Function called by Animation Event 'AttackBegin'
    public void AttackBegin()
    {
        Debug.Log("Chomper started attack animation");
    }

    // Function called by Animation Event 'AttackEnd'
    public void AttackEnd()
    {
        Debug.Log("Chomper ended attack animation");
    }
}
