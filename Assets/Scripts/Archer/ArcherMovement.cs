using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Animator))]
public class ArcherMovement : MonoBehaviour
{
    Movement moveScript;

    [System.Serializable]
    public class InputSettings
    {
        public string forwardInput = "Vertical";
        public string strafeInput = "Horizontal";
        public string sprintInput = "Sprint";
        public string aim = "Fire2";
        public string fire = "Fire1";
    }
    [SerializeField]
    public InputSettings input;

    [Header("Camera & Character Syncing")]
    public float lookDIstance = 5;
    public float lookSpeed = 5;

    [Header("Aiming Settings")]
    RaycastHit hit;
    public LayerMask aimLayers;
    Ray ray;

    [Header("Spine Settings")]
    public Transform spine;
    public Vector3 spineOffset;

    [Header("Head Rotation Settings")]
    public float lookAtPoint = 2.8f;

    [Header("Gravity Settings")]
    public float gravityValue = 1.2f;

    private Camera playerCamera;

    public Bow bowScript;
    bool isAiming;

    public bool testAim;

    bool hitDetected;

    Animator playerAnim;
    CharacterController cc;

    public Transform cameraTarget; // Assign to character's head/spine bone or create empty child
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;
    public float minLookAngle = -30f;
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;

    private float verticalRotation = 0;
    private float horizontalRotation = 0;

    Vector3 InitialCamPos;
    public LayerMask camCollisionLayers;

    public string AimingInput = "Fire2";

    public float originalFieldofView = 70;
    public float zoomFieldofView = 20;

    // Camera UICam;
    Transform center;

    // Start is called before the first frame update
    void Start()
    {
        if (cameraTarget == null)
        {
            GameObject target = new GameObject("CameraTarget");
            target.transform.SetParent(transform);
            target.transform.localPosition = new Vector3(0, cameraHeight, 0);
            cameraTarget = target.transform;
        }
        moveScript = GetComponent<Movement>();
        // camCenter = Camera.main.transform.parent;
        // mainCam = Camera.main.transform;
        playerAnim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        playerCamera = Camera.main;

        // UICam = playerCamera.GetComponentInChildren<Camera>();
        center = transform.GetChild(0);

        InitialCamPos = playerAnim.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis(input.forwardInput) != 0 || Input.GetAxis(input.strafeInput) != 0)
            RotateToCamView();

        if (!cc.isGrounded)
        {
            cc.Move(new Vector3(transform.position.x, transform.position.y - gravityValue, transform.position.z));
        }

        isAiming = Input.GetButton(input.aim);

        if (testAim)
            isAiming = true;

        if (bowScript.bowSettings.arrowCount < 1)
            isAiming = false;

        moveScript.AnimateCharacter(Input.GetAxis(input.forwardInput), Input.GetAxis(input.strafeInput));
        moveScript.SprintCharacter(Input.GetButton(input.sprintInput));
        moveScript.CharacterAim(isAiming);

        if (isAiming)
        {
            Aim();
            bowScript.EquipBow();

            if (bowScript.bowSettings.arrowCount > 0)
                moveScript.CharacterPullString(Input.GetButton(input.fire));

            if (Input.GetButtonUp(input.fire))
            {

                moveScript.CharacterFireArrow();
                if (hitDetected)
                {
                    bowScript.Fire(hit.point);
                }
                else
                {
                    bowScript.Fire(ray.GetPoint(300f));
                }
            }

        }
        else
        {
            bowScript.UnEquipBow();
            bowScript.RemoveCrosshair();
            DisableArrow();
            Release();
        }
        HandleCameraInput();
        ZoomCamera();
        HandleCamCollision();
    }

    void LateUpdate()
    {
        HandleCameraFollow();
        if (isAiming)
            RotateCharacterSpine();
    }

    void HandleCameraInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minLookAngle, maxLookAngle);
    }

    void ZoomCamera()
    {
        if (Input.GetButton(AimingInput))
        {
            Debug.Log("Zooming in while aiming");
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFieldofView, lookSpeed * Time.deltaTime);
            // UICam.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFieldofView, lookSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("Resetting zoom to original field of view");
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, originalFieldofView, lookSpeed * Time.deltaTime);
            // UICam.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, originalFieldofView, lookSpeed * Time.deltaTime);
        }
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
        }
    }

    void RotateToCamView()
    {
        Vector3 camCenterPos = playerCamera.transform.position;

        Vector3 lookPoint = camCenterPos + (playerCamera.transform.forward * lookDIstance);
        Vector3 direction = lookPoint - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation.x = 0;
        lookRotation.z = 0;

        Quaternion finalRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        transform.rotation = finalRotation;
    }

    //Does the aiming and sends a raycast to a target
    void Aim()
    {
        Vector3 camPosition = playerCamera.transform.position;
        Vector3 dir = playerCamera.transform.forward;

        ray = new Ray(camPosition, dir);
        if (Physics.Raycast(ray, out hit, 500f, aimLayers))
        {
            hitDetected = true;
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            bowScript.ShowCrosshair(hit.point);
        }
        else
        {
            hitDetected = false;
            bowScript.RemoveCrosshair();
        }
    }

    void RotateCharacterSpine()
    {
        RotateToCamView();
        spine.LookAt(ray.GetPoint(50));
        spine.Rotate(spineOffset);
    }


    public void Pull()
    {
        bowScript.PullString();
    }

    public void EnableArrow()
    {
        bowScript.PickArrow();
    }

    public void DisableArrow()
    {
        bowScript.DisableArrow();
    }

    public void Release()
    {
        bowScript.ReleaseString();
    }

    public void PlayPullSound()
    {
        bowScript.PullAudio();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (isAiming)
        {
            playerAnim.SetLookAtWeight(1f);
            playerAnim.SetLookAtPosition(ray.GetPoint(lookAtPoint));
        }
        else
        {
            playerAnim.SetLookAtWeight(0);
        }
    }
    
    void HandleCamCollision()
    {
        if (!Application.isPlaying)
            return;

        if(Physics.Linecast(transform.position + transform.up, playerCamera.transform.position, out hit, camCollisionLayers))
        {
            Vector3 newCamPos = new Vector3(hit.point.x + hit.normal.x * .2f, hit.point.y + hit.normal.y * .8f, hit.point.z + hit.normal.z * .2f);
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, newCamPos, Time.deltaTime * 0.1f);
        }
        else
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, InitialCamPos, Time.deltaTime * 0.1f);
        }

        Debug.DrawLine(transform.position + transform.up, playerCamera.transform.position, Color.blue);
    }
}