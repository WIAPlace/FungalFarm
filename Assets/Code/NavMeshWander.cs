using UnityEngine;
using UnityEngine.AI; // Required for NavMesh classes

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshWanderer : MonoBehaviour
{
    private static readonly int BlendHash = Animator.StringToHash("Blend");
    [Tooltip("How far the agent can look for a new position.")]
    [SerializeField] private float wanderRadius = 10f;
    
    [Tooltip("Minimum time to wait before picking a new destination.")]
    [SerializeField] private float minWaitTime = 1f;
    
    [Tooltip("Maximum time to wait before picking a new destination.")]
    [SerializeField] private float maxWaitTime = 3f;

    private NavMeshAgent agent;
    [SerializeField]private Animator animator;
    private float timer;
    private float currentWaitTime;


    void Start()
    {
        // Get the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
        
        // Pick the first destination immediately
        currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
        SetNewRandomDestination();
    }

    void Update()
    {
        // 1. Check if the agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;

            // 2. Wait until the timer hits the randomized wait time limit
            if (timer >= currentWaitTime)
            {
                SetNewRandomDestination();
                
                // Reset timer and pick a new random wait time for the next spot
                timer = 0f;
                currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
            }
            
        }
        float speed = agent.velocity.magnitude/agent.speed;

        animator.SetFloat(BlendHash, speed);
    }

    void SetNewRandomDestination()
    {
        // Get a random point within a sphere around the agent
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // Project that random point onto the nearest valid point on the NavMesh surface
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
}
