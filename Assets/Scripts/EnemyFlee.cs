using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFlee : MonoBehaviour
{
    // --- Inspector References ---
    [Header("References")]
    [SerializeField] private Transform player; // Changed to private with SerializeField
    [SerializeField] private Animator animator; // Changed to private with SerializeField

    // --- Inspector Distances ---
    [Header("Distances")]
    [Tooltip("Distance at which the enemy starts fleeing.")]
    [SerializeField] private float fleeDistance = 10f;
    [Tooltip("Distance at which the enemy is considered 'caught'.")]
    [SerializeField] private float caughtDistance = 1.5f;

    // --- Inspector Movement Speeds ---
    [Header("Movement Speeds")]
    [Tooltip("Speed when wandering randomly.")]
    [SerializeField] private float walkSpeed = 2f;
    [Tooltip("Speed when actively fleeing from the player.")]
    [SerializeField] private float runSpeed = 4f;

    // --- Inspector Random Walk Settings ---
    [Header("Random Walk Settings")]
    [Tooltip("Maximum radius for finding a random walk destination.")]
    [SerializeField] private float walkRadius = 5f;
    [Tooltip("Distance threshold for considering a random walk target reached.")]
    [SerializeField] private float randomWalkArrivalThreshold = 0.5f;

    // --- Fleeing Specific Settings ---
    [Header("Fleeing Settings")]
    [Tooltip("How far the agent will attempt to flee in a single step.")]
    [SerializeField] private float maxFleeStepDistance = 20f; // Max distance to look for a flee point
    [Tooltip("How many attempts to find a valid flee point, increasing distance each time.")]
    [SerializeField] private int fleeAttempts = 5;
    [Tooltip("Angle variation applied to the flee direction to make paths less predictable.")]
    [SerializeField] private float fleeAngleVariation = 60f; // Increased for more erratic fleeing

    // --- Private Internal State ---
    private NavMeshAgent agent;
    private bool isCaught = false;
    private Vector3 currentWalkTarget; // Renamed for clarity
    private bool isWalking = false;

    // --- Animator Parameter Hashes (More efficient than strings) ---
    private static readonly int AnimSpeedHash = Animator.StringToHash("Speed");
    private static readonly int AnimCaughtHash = Animator.StringToHash("Caught");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        // --- Player Reference Setup ---
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
            else
            {
                Debug.LogError("Player not found! Please assign the 'Player' transform in the Inspector or tag the player GameObject as 'Player'. Disabling EnemyFlee script.", this);
                enabled = false; // Disable script if essential components are missing
                return;
            }
        }

        // --- Animator Reference Setup ---
        if (animator == null)
        {
            Debug.LogError("Animator not assigned in the Inspector! Disabling EnemyFlee script.", this);
            enabled = false; // Disable script if essential components are missing
            return;
        }

        // Initialize animator
        animator.applyRootMotion = false;
        animator.SetFloat(AnimSpeedHash, 0f);
    }

    void Update()
    {
        // Early exit if caught or player is missing
        if (isCaught || player == null)
        {
            // Ensure agent is stopped if player is null and it's not caught
            if (agent.enabled && player == null)
            {
                agent.isStopped = true;
                agent.ResetPath();
                animator.SetFloat(AnimSpeedHash, 0f); // Set animation to idle
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < caughtDistance)
        {
            OnCaught();
            return;
        }
        else if (distanceToPlayer < fleeDistance)
        {
            // Fleeing state
            isWalking = false; // Stop random walking
            agent.speed = runSpeed;
            Flee();
        }
        else
        {
            // Random walking state
            agent.speed = walkSpeed;

            // Check if agent has arrived at currentWalkTarget or needs a new one
            // Using agent.remainingDistance and agent.pathPending for more robust checks
            if (!agent.pathPending && agent.remainingDistance < randomWalkArrivalThreshold)
            {
                if (GetRandomWalkPoint(out Vector3 newWalkTarget))
                {
                    currentWalkTarget = newWalkTarget;
                    agent.SetDestination(currentWalkTarget);
                    isWalking = true;
                }
                else
                {
                    // If no valid walk point found, stop walking for now
                    isWalking = false;
                    agent.ResetPath();
                    animator.SetFloat(AnimSpeedHash, 0f); // Set to idle if stuck
                }
            }
        }

        // Update animation speed based on velocity magnitude
        float speedNormalized = agent.velocity.magnitude / agent.speed;
        animator.SetFloat(AnimSpeedHash, speedNormalized);
    }

    /// <summary>
    /// Calculates a destination to flee away from the player and sets it for the NavMeshAgent.
    /// Uses an iterative approach to find a valid point along a deviated direction.
    /// </summary>
    private void Flee()
    {
        // Only set a new destination if the agent has reached its current one
        // or if it doesn't have a path, to prevent constant path recalculations
        if (agent.pathPending || (agent.hasPath && agent.remainingDistance > agent.stoppingDistance + 0.1f))
        {
            // If already on a path and not near the end, let it continue
            // This prevents the agent from recalculating a flee path every frame.
            return;
        }

        Vector3 directionAway = (transform.position - player.position).normalized;

        // Introduce some randomness to the flee direction for less predictable paths
        float angle = Random.Range(-fleeAngleVariation / 2f, fleeAngleVariation / 2f);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        Vector3 deviatedDirection = rotation * directionAway;

        NavMeshHit hit;
        Vector3 targetPoint = Vector3.zero;
        bool foundValidPoint = false;

        // Iteratively try to find a valid point further away
        for (int i = 0; i < fleeAttempts; i++)
        {
            // Increase search distance with each attempt
            float currentSearchDistance = fleeDistance + (maxFleeStepDistance - fleeDistance) * ((float)i / (fleeAttempts - 1));
            Vector3 proposedTarget = transform.position + deviatedDirection * currentSearchDistance;

            // Sample the NavMesh around the proposed target
            // The search radius for SamplePosition should be reasonable, like agent.height or a small fixed value
            if (NavMesh.SamplePosition(proposedTarget, out hit, agent.height * 2f, NavMesh.AllAreas))
            {
                // Verify the found point is actually reachable
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    targetPoint = hit.position;
                    foundValidPoint = true;
                    Debug.DrawLine(transform.position, hit.position, Color.green, 0.5f); // Draw valid path in green
                    break; // Found a valid and reachable point, break the loop
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.position, Color.red, 0.5f); // Draw unreachable sampled point in red
                }
            }
            else
            {
                // If SamplePosition itself fails, draw a conceptual line
                Debug.DrawLine(transform.position, proposedTarget, Color.blue, 0.5f);
            }
        }

        if (foundValidPoint)
        {
            agent.SetDestination(targetPoint);
        }
        else
        {
            Debug.LogWarning($"Couldn't find a valid flee destination for {gameObject.name} after {fleeAttempts} attempts. Agent may stop.", this);
            agent.ResetPath(); // Clear path if no destination is found
            animator.SetFloat(AnimSpeedHash, 0f); // Set to idle animation
        }
    }

    /// <summary>
    /// Handles the enemy being caught by the player.
    /// Disables NavMeshAgent, triggers "Caught" animation, and snaps to ground.
    /// </summary>
    public void OnCaught()
    {
        if (isCaught) return; // Already caught, prevent re-entry
        isCaught = true;

        if (agent != null)
        {
            agent.enabled = false; // Disabling the agent also stops it and clears its path
        }

        SnapToGround(); // Attempt to snap to ground after disabling agent

        animator.applyRootMotion = true; // Allow animator to control movement
        animator.SetFloat(AnimSpeedHash, 0f); // Ensure idle animation
        animator.SetTrigger(AnimCaughtHash); // Trigger caught animation
    }

    /// <summary>
    /// Snaps the enemy's Y position to the ground using a Raycast.
    /// </summary>
    private void SnapToGround()
    {
        // Define ground layers for the raycast to be more precise
        // Make sure your ground/NavMesh layers are included here
        int groundLayerMask = LayerMask.GetMask("Ground", "NavMesh"); // Example layer masks

        // Raycast down from slightly above the enemy's current position
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 5f, groundLayerMask))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    /// <summary>
    /// Attempts to find a random walkable point on the NavMesh within the specified walkRadius.
    /// </summary>
    /// <param name="result">The found random walk point, if successful.</param>
    /// <returns>True if a valid point was found, false otherwise.</returns>
    private bool GetRandomWalkPoint(out Vector3 result)
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        Vector3 randomPoint = transform.position + randomDirection; // Calculate a point within the sphere around the enemy

        // Sample the NavMesh around the random point
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw spheres for visualization in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, caughtDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkRadius);

        // Draw walk target if currently walking
        if (isWalking && agent != null && agent.hasPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(agent.destination, 0.2f); // Draw a small sphere at the destination
            Gizmos.DrawLine(transform.position, agent.destination); // Draw a line to the destination
        }
    }
}