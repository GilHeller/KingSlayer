using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFlee : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Distances")]
    public float fleeDistance = 10f;
    public float caughtDistance = 1.5f;

    [Header("Movement Speeds")]
    public float walkSpeed = 2f;  // Speed during random walk
    public float runSpeed = 4f;   // Speed when fleeing

    [Header("Random Walk Settings")]
    public float walkRadius = 5f;

    private NavMeshAgent agent;
    private bool isCaught = false;
    private Vector3 walkTarget;
    private bool isWalking = false;

    // Animator parameter names
    private const string ANIM_SPEED = "Speed";
    private const string ANIM_CAUGHT = "Caught";

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
            else
                Debug.LogError("Player not found! Tag the player GameObject as 'Player'.");
        }

        if (animator == null)
            Debug.LogError("Animator not assigned!");

        animator.applyRootMotion = false;
        animator.SetFloat(ANIM_SPEED, 0f);
    }

    void Update()
    {
        if (isCaught || player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < caughtDistance)
        {
            OnCaught();
            return;
        }

        if (distanceToPlayer < fleeDistance)
        {
            // Fleeing
            isWalking = false;
            agent.speed = runSpeed;
            Flee();
        }
        else
        {
            // Random walking
            agent.speed = walkSpeed;

            if (!isWalking || Vector3.Distance(transform.position, walkTarget) < 0.5f)
            {
                if (GetRandomWalkPoint(out Vector3 newWalkTarget))
                {
                    walkTarget = newWalkTarget;
                    agent.SetDestination(walkTarget);
                    isWalking = true;
                }
            }
        }

        // Update animation speed based on velocity magnitude (normalized)
        float speedNormalized = agent.velocity.magnitude / agent.speed;
        animator.SetFloat(ANIM_SPEED, speedNormalized);
    }

    void Flee()
    {
        Vector3 directionAway = (transform.position - player.position).normalized;

        float angle = Random.Range(-30f, 30f); // random deviation angle
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        Vector3 deviatedDirection = rotation * directionAway;

        Vector3 targetPos = transform.position + deviatedDirection * fleeDistance;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, fleeDistance * 2, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.DrawLine(transform.position, hit.position, Color.cyan);
        }
        else
        {
            Debug.LogWarning("Couldn't find a valid flee destination.");
        }
    }

    public void OnCaught()
    {
        if (isCaught) return;
        isCaught = true;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        SnapToGround();

        animator.applyRootMotion = true;
        animator.SetFloat(ANIM_SPEED, 0f);
        animator.SetTrigger(ANIM_CAUGHT);
    }

    private void SnapToGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    bool GetRandomWalkPoint(out Vector3 result)
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, caughtDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkRadius);
    }
}
