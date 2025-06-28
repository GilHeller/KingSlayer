using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public float normalDistance = 5f;
    public float aimDistance = 2f;
    public float normalFOV = 60f;
    public float aimFOV = 30f;
    public float transitionSpeed = 2f;
    
    private Camera cam;
    private bool isAiming = false;
    private Vector3 originalPosition;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        originalPosition = transform.localPosition;
    }
    
    void Update()
    {
        if (target != null)
        {
            UpdateCameraPosition();
            UpdateCameraFOV();
        }
    }
    
    void UpdateCameraPosition()
    {
        float targetDistance = isAiming ? aimDistance : normalDistance;
        Vector3 targetPosition = target.position - target.forward * targetDistance + Vector3.up * 1.5f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
    
    void UpdateCameraFOV()
    {
        float targetFOV = isAiming ? aimFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * transitionSpeed);
    }
    
    public void EnterAimMode()
    {
        isAiming = true;
    }
    
    public void ExitAimMode()
    {
        isAiming = false;
    }
}