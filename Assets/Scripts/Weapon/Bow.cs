using UnityEngine;

public class Bow : Weapon
{
    [Header("Bow Specific")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float maxDrawTime = 2f;
    public float maxArrowSpeed = 50f;
    
    private bool isAiming = false;
    private float drawTime = 0f;
    private CameraController cameraController;
    
    protected override void Start()
    {
        base.Start();
        cameraController = FindObjectOfType<CameraController>();
    }
    
    public override void Attack()
    {
        if (isAiming)
        {
            FireArrow();
        }
    }
    
    public override void StartAiming()
    {
        isAiming = true;
        drawTime = 0f;
        if (cameraController != null)
        {
            cameraController.EnterAimMode();
        }
        Debug.Log("Started aiming bow");
    }
    
    public override void StopAiming()
    {
        if (isAiming)
        {
            FireArrow();
        }
        isAiming = false;
        if (cameraController != null)
        {
            cameraController.ExitAimMode();
        }
    }
    
    void Update()
    {
        if (isAiming)
        {
            drawTime += Time.deltaTime;
            drawTime = Mathf.Clamp(drawTime, 0f, maxDrawTime);
        }
    }
    
    void FireArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
            
            float drawPower = drawTime / maxDrawTime;
            float arrowSpeed = maxArrowSpeed * drawPower;
            
            arrowRb.linearVelocity = arrowSpawnPoint.forward * arrowSpeed;
            
            Debug.Log($"Fired arrow with {drawPower * 100}% power");
        }
        
        drawTime = 0f;
    }
}