using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolScript : MonoBehaviour
{
    [Header("Waypoints & Movement")]
    public Transform[] wayPoints;
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 4.0f;
    public float rotationSpeed = 5.0f;
    public float gravity = 9.81f;

    [Header("Vision Settings")]
    public float visionDistance = 10f;
    public float visionAngle = 45f;
    private Transform player;

    private int currentWayPoint = 0;
    private CharacterController controller;
    private Vector3 velocity;
    private bool chasing = false;

    [Header("Combat Settings")]
    public float punchDistance = 1.5f;
    public float punchCooldown = 1.0f;
    public int damageAmount = 10;

    private float lastPunchTime = -Mathf.Infinity;

    [Header("Animation")]
    private Animator animator;

    private EnemyHealth patrolHealth;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        patrolHealth = GetComponent<EnemyHealth>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (wayPoints.Length == 0 || player == null) return;
        if (patrolHealth != null && patrolHealth.currentHealth <= 0 ) return;

        if (player == null)
        {
            Debug.LogWarning("Player not found, trying to find by tag.");
            // Try to find the player by tag if not assigned
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null) return;
        }

        DetectPlayer();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (chasing && distanceToPlayer <= punchDistance)
        {
            AttemptPunch();
            return;
        }

        Vector3 moveDirection = chasing ? GetChaseDirection() : GetPatrolDirection();

        ApplyGravity();
        MoveCharacter(moveDirection);
        RotateCharacter(moveDirection);
    }


    void AttemptPunch()
    {
        if (Time.time - lastPunchTime >= punchCooldown)
        {
            animator.SetTrigger("Punch");
            lastPunchTime = Time.time;

            if (player != null)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                }
            }
        }

        // Stop movement animations
        animator.SetBool("Run", false);
        animator.SetBool("Walk", false);
    }


    void DetectPlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;

        float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);

        if (toPlayer.magnitude <= visionDistance && angleToPlayer <= visionAngle / 2f)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 1.5f, toPlayer.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, visionDistance))
            {
                if (hit.transform == player)
                {
                    chasing = true;
                    return;
                }
            }
        }

        chasing = false;
    }

    Vector3 GetChaseDirection()
    {
        animator.SetBool("Run", true);
        animator.SetBool("Walk", false);

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        return dir.normalized * chaseSpeed;
    }
    Vector3 GetPatrolDirection()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Walk", true);
        Vector3 target = wayPoints[currentWayPoint].position;
        Vector3 dir = target - transform.position;
        dir.y = 0;

        if (dir.magnitude < 0.3f)
        {
            currentWayPoint = (currentWayPoint + 1) % wayPoints.Length;
            return Vector3.zero;
        }

        return dir.normalized * patrolSpeed;
    }

    void ApplyGravity()
    {
        if (!controller.isGrounded)
            velocity.y -= gravity * Time.deltaTime;
        else
            velocity.y = -1f;
    }

    void MoveCharacter(Vector3 moveDirection)
    {
        Vector3 finalMove = moveDirection;
        finalMove.y = velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    void RotateCharacter(Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero) return;

        Quaternion toRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 forward = transform.forward;
        Quaternion leftRayRotation = Quaternion.Euler(0, -visionAngle / 2f, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, visionAngle / 2f, 0);

        Vector3 leftRayDir = leftRayRotation * forward;
        Vector3 rightRayDir = rightRayRotation * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, leftRayDir * visionDistance);
        Gizmos.DrawRay(origin, rightRayDir * visionDistance);
    }

}
