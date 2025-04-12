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
    
    [Header("Chase Settings")]
    public float lostDistance = 10f;        // If target is farther than this, give up chasing

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex;
    private float waitTimer;
    public List<Transform> patrolPoints;    // Generated patrol points

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    // Flag indicating whether Ellen has been detected ("found").
    private bool targetFound = false;

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

        // If chasing or attacking but the target is too far away, give up and return to patrol.
        if ((currentState == State.Chase || currentState == State.Attack) && distanceToTarget > lostDistance)
        {
            targetFound = false;
            currentState = State.Patrol;
            animator.SetBool("isAttacking", false);
            // Optionally, reset destination to the patrol point.
            if (patrolPoints != null && patrolPoints.Count > 0)
            {
                agent.destination = patrolPoints[currentPatrolIndex].position;
            }
        }

        // In Chase and Attack states, always update the destination to the target.
        if (currentState == State.Chase || currentState == State.Attack)
        {
            agent.destination = target.position;
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase(distanceToTarget);
                break;

            case State.Attack:
                // If target moves out of attack distance but is still within lostDistance, revert to chasing.
                if (distanceToTarget > attackDistance)
                {
                    currentState = State.Chase;
                    animator.SetBool("isAttacking", false);
                }
                else
                {
                    // Continue attacking while following the target.
                    animator.SetBool("isAttacking", true);
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

        // Patrol behavior: move among the patrol points.
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
        
        // Continuously update destination toward the target.
        agent.destination = target.position;
        
        // When the target is within attack distance, switch to the Attack state.
        if (distanceToTarget <= attackDistance)
        {
            currentState = State.Attack;
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.name);
        if (other.CompareTag("Player"))
        {
            // Once the target is "found", set the flag and switch to Chase.
            targetFound = true;
            currentState = State.Chase;
            Debug.Log("Chomper: Target found at " + Time.time);
        }
    }

    // Instead of immediately returning to patrol on OnTriggerExit,
    // we now log the event and let the "lostDistance" condition handle resetting the state.
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Chomper: Target left trigger at " + Time.time);
            // Optionally, you might choose to set a timer here before giving up chasing
            // or simply rely on lostDistance in Update.
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
