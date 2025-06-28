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
    public float safeDistance = 20f;

    private NavMeshAgent agent;
    private float distanceToPlayer;
    private bool isCaught = false;

    // Animator parameter names
    private const string ANIM_RUNNING = "Running";
    private const string ANIM_CAUGHT = "Caught";

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Auto-find player if not assigned
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
        animator.SetBool(ANIM_RUNNING, false);
    }

    void Update()
    {
        if (isCaught || player == null)
            return;

        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < caughtDistance)
        {
            OnCaught();
            return;
        }

        if (distanceToPlayer < fleeDistance)
        {
            Flee();
            animator.SetBool(ANIM_RUNNING, true);
        }
        else if (agent.remainingDistance < 0.1f)
        {
            if (agent.hasPath) agent.ResetPath();
            animator.SetBool(ANIM_RUNNING, false);
        }
    }

    void Flee()
    {
        Vector3 directionAway = (transform.position - player.position).normalized;

        float angle = Random.Range(-30f, 30f); // Adjust ±angle as needed
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
        animator.SetBool(ANIM_RUNNING, false);
        animator.SetTrigger(ANIM_CAUGHT);
    }


    // Optional: snap Y position to ground after caught (if animation floats)
    private void SnapToGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    // Debug: show distances in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, caughtDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
