using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float crouchSpeed = 1f;
    public float jumpForce = 8f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -15f;

    [Header("Camera Settings")]
    public Transform cameraTarget; // Assign to character's head/spine bone or create empty child
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;
    public float minLookAngle = -30f;
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;

    [Header("Stealth Settings")]
    public bool isCrouching = false;
    public bool isHidden = false;
    public float noiseLevel = 1f; // 0 = silent, 1 = normal, 2 = loud

    [Header("Mixamo Animation Settings")]
    public Animator animator;
    [Header("Animation Parameter Names (match your Mixamo setup)")]
    public string speedParam = "Speed";
    public string crouchParam = "Sneak";
    public string jumpParam = "Jump";
    public string runParam = "Run";
    public string walkParam = "Walk";
    public string punchParam = "Punch"; // New: Animation parameter for punching

    [Header("Punch Settings")]
    public float punchDamage = 10f;
    public float punchRange = 1f; // How far the punch reaches
    public float punchRadius = 0.3f; // Radius of the punch detection sphere
    public float punchCooldown = 0.5f; // Time between punches
    public LayerMask punchableLayers; // Layers that can be hit by a punch (e.g., "Enemy")
    public Transform punchOrigin; // Optional: Create an empty GameObject at the character's fist

    [Header("Ground Detection")]
    public Transform groundCheck; // Create empty child at character's feet
    public float groundDistance = 0.2f;
    public LayerMask groundMask = 1; // Ground layer

    [Header("Replaceable Objects")]
    public GameObject CubePrefab;
    public GameObject CubePrefab2;

    private Camera playerCamera;
    private CharacterController characterController;
    private float verticalRotation = 0;
    private float horizontalRotation = 0;
    private float turnSmoothVelocity;
    private bool isGrounded;
    private Vector3 velocity = Vector3.zero ;
    private float originalHeight;

    // Animation state tracking
    private bool wasMoving = false;
    private float animationSpeed = 0f;

    // Punch related
    private float lastPunchTime;
    private bool canPunch = true;
    

    void Start()
    {
        // Get or create character controller (better for character movement than Rigidbody)

        if (GameManager.Instance.gameData.spawnPoint != Vector3.zero)
        {
            gameObject.transform.position = GameManager.Instance.gameData.spawnPoint; // Default spawn point
        } else
        {
            GameManager.Instance.gameData.spawnPoint = transform.position; // Set initial spawn point
        }
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.radius = 0.3f;
            characterController.height = 1.8f;
            characterController.center = new Vector3(0, 0.9f, 0);
        }
        originalHeight = characterController.height;

        playerCamera = Camera.main;

        // Auto-find animator if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();

        // Create camera target if not assigned
        if (cameraTarget == null)
        {
            GameObject target = new GameObject("CameraTarget");
            target.transform.SetParent(transform);
            target.transform.localPosition = new Vector3(0, cameraHeight, 0);
            cameraTarget = target.transform;
        }

        // Create ground check if not assigned
        if (groundCheck == null)
        {
            GameObject groundChecker = new GameObject("GroundCheck");
            groundChecker.transform.SetParent(transform);
            groundChecker.transform.localPosition = new Vector3(0, 0.1f, 0);
            groundCheck = groundChecker.transform;
        }

        // Create punch origin if not assigned (default to a point in front of the character)
        if (punchOrigin == null)
        {
            GameObject punchOriginGO = new GameObject("PunchOrigin");
            punchOriginGO.transform.SetParent(transform);
            punchOriginGO.transform.localPosition = new Vector3(0, 1.2f, 0.5f); // Adjust as needed
            punchOrigin = punchOriginGO.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameData != null)
        {
            if (GameManager.Instance.gameData.spawnPoint != Vector3.zero && GameManager.Instance.gameData.isNewScene)
            {
                transform.position = GameManager.Instance.gameData.spawnPoint;
            }
        }
    
        GameManager.Instance.gameData.activePlayer = gameObject; // Set this player as the active player in GameData
}



    void Update()
    {

        if (GameManager.Instance.gameData.spawnPoint != Vector3.zero && GameManager.Instance.gameData.isNewScene)
        {
            gameObject.transform.position = GameManager.Instance.gameData.spawnPoint; // Default spawn point
        }

        GameManager.Instance.gameData.isNewScene = false;


        if (!gameObject.name.ToLower().Contains("archer"))
        {
            HandleGroundCheck();
            HandleCrouch();
            HandleMovement();
            HandleCameraInput();
            HandlePunchInput(); // New: Handle punch input
            HandleAnimations();
        }
    }

    void LateUpdate()
    {
        HandleCameraFollow();
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep grounded
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate target angle based on camera and input
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move in the direction the character is facing
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
            float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

            // Use CharacterController.Move instead of Rigidbody
            characterController.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // Calculate noise level for stealth
            noiseLevel = isCrouching ? 0.3f : (isRunning ? 1.5f : 1f);

            // Store movement state for animations
            wasMoving = true;
            animationSpeed = currentSpeed;
        }
        else
        {
            noiseLevel = 0f;
            wasMoving = false;
            animationSpeed = 0f;
        }

        // Handle gravity
        velocity.y += gravity * Time.deltaTime;

        Debug.Log("Velocity: " + velocity);

        Debug.Log("CharacterController: " + characterController);
        Debug.Log("CharacterController.isGrounded: " + characterController.isGrounded);
        characterController.Move(velocity * Time.deltaTime);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            if (animator != null)
                animator.SetTrigger(jumpParam);
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            // Adjust character controller
            characterController.height = isCrouching ? originalHeight * 0.6f : originalHeight;
            characterController.center = new Vector3(0, characterController.height * 0.5f, 0);

            // Adjust camera target height
            Vector3 targetPos = cameraTarget.localPosition;
            targetPos.y = isCrouching ? cameraHeight * 0.7f : cameraHeight;
            cameraTarget.localPosition = targetPos;
        }
    }

    void HandleCameraInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minLookAngle, maxLookAngle);
    }

    void HandleCameraFollow()
    {
        if (playerCamera != null && cameraTarget != null)
        {
            // Calculate desired camera position
            Vector3 targetPosition = cameraTarget.position;
            Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
            Vector3 direction = rotation * Vector3.back;

            // Raycast to check for obstacles
            RaycastHit hit;
            float distance = cameraDistance;
            if (Physics.Raycast(targetPosition, direction, out hit, cameraDistance))
            {
                distance = hit.distance - 0.1f; // Small offset to prevent clipping
            }

            Vector3 desiredPosition = targetPosition + direction * distance;

            // Smooth camera movement
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, desiredPosition, Time.deltaTime * 10f);
            playerCamera.transform.LookAt(cameraTarget);
            HandleZoom();
        }
    }

    void HandleZoom()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                playerCamera.fieldOfView -= scroll * 10f; // Adjust zoom speed as needed
                playerCamera.fieldOfView = Mathf.Clamp(playerCamera.fieldOfView, 20f, 150f); // Clamp zoom limits
            }
        }
    }

    void HandlePunchInput()
{
    // Check for left mouse button click and if cooldown allows
    if (Input.GetMouseButtonDown(0) && canPunch)
    {
        Hit();
        canPunch = false; // Start cooldown
        lastPunchTime = Time.time;
    }

    // Reset canPunch after cooldown
    if (!canPunch && Time.time >= lastPunchTime + punchCooldown)
    {
        canPunch = true;
    }
}

    public void Hit(float externalDamage = 0f)
    {
        if (animator != null && !externalDamage.Equals(0f)) // If external damage is provided, use it
        {
            punchDamage = externalDamage;
        } else {
            animator.SetTrigger(punchParam); // Trigger the punch animation
        }

        Vector3 origin = (punchOrigin != null) ? punchOrigin.position : transform.position + transform.forward * 0.5f;

        RaycastHit[] hits = Physics.SphereCastAll(origin, punchRadius, transform.forward, punchRange, punchableLayers);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == transform || hit.collider.isTrigger) continue;

            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();

            Debug.Log("EnemyHealth: " + enemyHealth);

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(punchDamage);
                Debug.Log($"Hit {hit.collider.name} for {punchDamage} damage!");
                // You might add effects here like particle systems, sound effects
            }
            else if (CubePrefab && hit.collider.name == CubePrefab?.name)
            {
                Debug.Log("Replace object");
                CubePrefab.GetComponent<ReplaceWithOther>()?.ReplaceNow();
            }
            else if (CubePrefab2 && hit.collider.name == CubePrefab2?.name)
            {
                Debug.Log("Replace object");
                CubePrefab2.GetComponent<ReplaceWithOther>()?.ReplaceNow();
            }
            else
            {
                Debug.Log($"Punch hit {hit.collider.name}, but no Health component found.");
            }
        }
    }

    void HandleAnimations()
    {
        if (animator == null) return;

        // Set movement parameters
        float speedValue = wasMoving ? (animationSpeed / runSpeed) : 0f; // Normalize speed (0-1)
        animator.SetFloat(speedParam, speedValue);

        // Set state booleans
        animator.SetBool(crouchParam, isCrouching);
        animator.SetBool(runParam, Input.GetKey(KeyCode.LeftShift) && wasMoving && !isCrouching);
        animator.SetBool(walkParam, wasMoving && !isCrouching);
        // Ground state (useful for landing animations)
        animator.SetBool("IsGrounded", isGrounded);

        // Additional parameters you might want to add to your Mixamo animations
        animator.SetFloat("VelocityY", velocity.y);
        animator.SetFloat("InputMagnitude", wasMoving ? 1f : 0f);
    }

    // Get camera-relative input direction (useful for other systems)
    public Vector3 GetCameraRelativeDirection()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        return (forward * vertical + right * horizontal).normalized;
    }

    // Utility methods for other scripts
    public bool IsMoving() { return wasMoving; }
    public bool IsRunning() { return Input.GetKey(KeyCode.LeftShift) && wasMoving && !isCrouching; }
    public float GetCurrentSpeed() { return animationSpeed; }

    void OnDrawGizmosSelected()
    {
        // Visualize ground check
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

        // Visualize punch range
        if (punchOrigin != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(punchOrigin.position, punchRadius); // Origin sphere
            Gizmos.DrawWireSphere(punchOrigin.position + transform.forward * punchRange, punchRadius); // End sphere
            Gizmos.DrawLine(punchOrigin.position, punchOrigin.position + transform.forward * punchRange); // Connecting line
        }
    }
}