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
    
    [Header("Punch Sound Settings")]
    public AudioClip punchSound; // The sound clip for punching
    private AudioSource audioSource;

    [Header("Ground Detection")]
    public Transform groundCheck; // Create empty child at character's feet
    public float groundDistance = 0.2f;
    public LayerMask groundMask = 1; // Ground layer

    [Header("Replaceable Objects")]
    public GameObject CubePrefab;
    public GameObject CubePrefab2;

    [Header("Throwing Settings")]
    public GameObject projectilePrefab; // The object you want to throw (e.g., a rock, a grenade)
    public Transform throwOrigin; // Point from which the projectile is thrown (e.g., character's hand)
    public float throwForce = 15f; // How fast the projectile is launched
    public float aimFOV = 40f; // Camera field of view when aiming
    public float throwCooldown = 1.0f; // Time between throws
    public float throwAnimationTime = 0.5f; // The duration of the throw animation

    [Header("Aiming Settings")]
    public float aimingTurnSpeed = 5f; // How fast the character turns while aiming
    public bool isAiming = false;
    private float lastThrowTime;
    private float originalFOV;
    private bool canThrow = true;
    public string aimParam = "isAiming";
    public float lookDIstance = 5;
    public float lookSpeed = 5;

    [Header("Crosshair Settings")]
    public GameObject crossHairPrefab;
    GameObject currentCrossHair;

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

        // Store original camera FOV
        if (playerCamera != null)
        {
            originalFOV = playerCamera.fieldOfView;
        }

        // Create throw origin if not assigned (default to a point in front of the character's chest)
        if (throwOrigin == null)
        {
            GameObject throwOriginGO = new GameObject("ThrowOrigin");
            throwOriginGO.transform.SetParent(transform);
            throwOriginGO.transform.localPosition = new Vector3(0, 1.4f, 0.5f);
            throwOrigin = throwOriginGO.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    public void OnEnable()
    {
        Debug.Log("OnEnable called for ThirdPersonController");
        if (GameManager.Instance == null || GameManager.Instance.gameData == null)
        {
            Debug.LogWarning("GameManager not ready yet. Skipping OnEnable logic.");
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.gameData != null)
        {
            if (GameManager.Instance.gameData.spawnPoint != Vector3.zero)
            {
                Debug.Log("Setting spawn point to: " + GameManager.Instance.gameData.spawnPoint);
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


        //if (!gameObject.name.ToLower().Contains("archer"))
        //{
        HandleGroundCheck();
        HandleCrouch();
        HandleAimingAndThrowing();
        RotateTowardsCursor();
        HandleMovement();
        HandleCameraInput();
        HandlePunchInput(); // New: Handle punch input
        HandleAnimations();
        //}
    }

    void RotateTowardsCursor()
    {
        if (!isAiming || playerCamera == null) return;

        // Create a ray from camera through the cursor
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position); // y=player height plane

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f; // keep only horizontal rotation

            if (direction.sqrMagnitude > 0.001f)
            {
                // Smooth rotation
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            ShowCrosshair(hitPoint);
        }
    }

    public void ShowCrosshair(Vector3 crosshairPos)
    {
        if (!currentCrossHair)
            currentCrossHair = Instantiate(crossHairPrefab);

        currentCrossHair.transform.position = crosshairPos;
        currentCrossHair.transform.LookAt(Camera.main.transform);
    }


    void HandleAimingAndThrowing()
    {
        // Aiming
        if (Input.GetMouseButtonDown(1)) // Right mouse button down
        {
            isAiming = true;

            if (animator != null)
            {
                animator.SetBool(aimParam, true);
            }
            // Adjust camera FOV
            //if (playerCamera != null)
            //{
            //    playerCamera.fieldOfView = aimFOV;
            //}
        }
        else if (Input.GetMouseButtonUp(1)) // Right mouse button up
        {
            isAiming = false;
            // Reset camera FOV

            if (animator != null)
            {
                animator.SetBool(aimParam, false);
            }

            //if (playerCamera != null)
            //{
            //    playerCamera.fieldOfView = originalFOV;
            //}
        }

        ZoomCamera();

    void ZoomCamera()
    {
        float targetFOV = Input.GetMouseButtonDown(1) ? aimFOV : originalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, lookSpeed * Time.deltaTime);
    }

        // Throwing
        if (isAiming && Input.GetMouseButtonDown(0) && canThrow) // Left mouse button while aiming
        {
            ThrowObject();
            canThrow = false;
            lastThrowTime = Time.time;
        }

        // Cooldown for throwing
        if (!canThrow && Time.time >= lastThrowTime + throwCooldown)
        {
            canThrow = true;
        }
    }

    public void ThrowObject()
    {
        if (projectilePrefab == null || throwOrigin == null)
        {
            Debug.LogWarning("Projectile prefab or throw origin not set!");
            return;
        }

        // Instantiate the projectile at the throw origin's position and rotation
        GameObject projectile = Instantiate(projectilePrefab, throwOrigin.position, throwOrigin.rotation);

        // Get the Rigidbody component and add force
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Get the direction from the camera's forward vector
            Vector3 throwDirection = playerCamera.transform.forward;
            rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        }

        // Trigger the throw animation (you'll need to create a new parameter in your Animator)
        if (animator != null)
        {
            animator.SetTrigger("Throw"); // Replace "Throw" with your animation parameter name
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
            // If aiming, turn the character to face the camera direction
            if (isAiming)
            {
                // Smoothly rotate the character to face the same direction as the camera
                Quaternion targetRotation = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimingTurnSpeed * Time.deltaTime);
            } else { 
                // Calculate target angle based on camera and input
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            // Move in the direction the character is facing
            //Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 moveDir = transform.forward; // Change this to use the character's current forward vector

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
        if (isAiming)
        {
            return; // Exit the method immediately
        }
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

        if (punchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(punchSound);
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
