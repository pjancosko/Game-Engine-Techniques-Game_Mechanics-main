using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChomperAI : MonoBehaviour
{
    [Header("Target & Patrol Settings")]
    public Transform target;              // Assign Ellen's transform in the Inspector
    public int numberOfPatrolPoints = 3;    // How many patrol points to generate
    public float patrolRadius = 5f;         // Patrol radius around the spawn position
    public float patrolWaitTime = 2f;       // Time to wait at each patrol point
    public float attackDistance = 2f;       // Distance within which the Chomper will attack

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex;
    private float waitTimer;
    public List<Transform> patrolPoints; // Generated patrol points

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    // Flag indicating whether Ellen is in contact via trigger.
    private bool isInContact = false;

    void Start()
    {
        // Get required components.
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent component found on Chomper!");
            return;
        }
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found on Chomper!");
        }

        // Ensure the Chomper spawns on the NavMesh.
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            Debug.LogError("Chomper is not close enough to a NavMesh!");
        }

        // Generate patrol points around this spawn position.
        patrolPoints = GeneratePatrolPoints(transform.position, numberOfPatrolPoints, patrolRadius);

        // Set the first patrol destination if available.
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            currentPatrolIndex = 0;
            agent.destination = patrolPoints[currentPatrolIndex].position;
        }
        else
        {
            Debug.LogWarning("No patrol points generated.");
        }

        // Start in Patrol state.
        currentState = State.Patrol;
    }

    void Update()
{
    if (target == null)
    {
        Debug.LogError("Target (Ellen) is not assigned to the Chomper!");
        return;
    }

    float distanceToTarget = Vector3.Distance(transform.position, target.position);

    switch (currentState)
    {
        case State.Patrol:
            Patrol();
            // Switch to Chase if Ellen is in contact.
            if (isInContact)
            {
                currentState = State.Chase;
            }
            break;
        case State.Chase:
            Chase(distanceToTarget);
            break;
        case State.Attack:
            if (distanceToTarget > attackDistance)
            {
                currentState = State.Chase;
                agent.isStopped = false;
                animator.SetBool("Attack", false);
            }
            break;
    }
}

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points available.");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent is not on NavMesh during patrol.");
            return;
        }

        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                waitTimer = 0;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                agent.destination = patrolPoints[currentPatrolIndex].position;
            }
        }
    }

    void Chase(float distanceToTarget)
    {
        if (target == null)
        {
            Debug.LogError("Target is not assigned!");
            return;
        }

        // Set destination to target's position.
        agent.destination = target.position;

        if (distanceToTarget <= attackDistance)
        {
            currentState = State.Attack;
            agent.isStopped = true;
            animator.SetBool("Attack", true);
        }
        else
        {
            agent.isStopped = false;
            animator.SetBool("Attack", false);
        }
    }

    void OnTriggerEnter(Collider other)
{
    Debug.Log("OnTriggerEnter called with: " + other.name);
    if (other.CompareTag("Player"))
    {
        isInContact = true;
        currentState = State.Chase;
        Debug.Log("Chomper: Ellen entered contact at " + Time.time);
    }
}

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInContact = false;
            currentState = State.Patrol;
            Debug.Log("Chomper: Ellen exited contact at " + Time.time);
            if (patrolPoints != null && patrolPoints.Count > 0)
            {
                agent.destination = patrolPoints[currentPatrolIndex].position;
            }
        }
    }

    List<Transform> GeneratePatrolPoints(Vector3 center, int numberOfPoints, float radius)
    {
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
            Vector3 randomPos = center + randomOffset;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
            {
                GameObject patrolPoint = new GameObject("PatrolPoint");
                patrolPoint.transform.position = hit.position;
                points.Add(patrolPoint.transform);
            }
        }
        return points;
    }
}
