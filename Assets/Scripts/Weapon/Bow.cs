using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [System.Serializable]
    public class BowSettings
    {
        [Header("Arrow Settings")]
        public float arrowCount;
        public Rigidbody arrowPrefab;
        public Transform arrowPos;
        public Transform arrowEquipParent;
        public float arrowForce = 3;

        public float arrowSpeed = 50f; // Speed of the arrow when fired

        [Header("Bow Equip & UnEquip Settings")]
        public Transform EquipPos;
        public Transform UnEquipPos;

        public Transform UnEquipParent;
        public Transform EquipParent;

        [Header("Bow String Settings")]
        public Transform bowString;
        public Transform stringInitialPos;
        public Transform stringHandPullPos;
        public Transform stringInitialParent;

        [Header("Bow Audio Settings")]
        public AudioClip pullStringAudio;
        public AudioClip releaseStringAudio;
        public AudioClip drawArrowAudio;
    }
    [SerializeField]
    public BowSettings bowSettings;

    [Header("Crosshair Settings")]
    public GameObject crossHairPrefab;
    GameObject currentCrossHair;

    Rigidbody currentArrow;

    bool canPullString = false;
    bool canFireArrow = false;

    AudioSource bowAudio;

    // Start is called before the first frame update
    void Start()
    {
        bowAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickArrow()
    {
        bowAudio.PlayOneShot(bowSettings.drawArrowAudio);
        bowSettings.arrowPos.gameObject.SetActive(true);
    }

    public void DisableArrow()
    {
        bowSettings.arrowPos.gameObject.SetActive(false);
    }

    public void PullString()
    {
        bowSettings.bowString.transform.position = bowSettings.stringHandPullPos.position;
        bowSettings.bowString.transform.parent = bowSettings.stringHandPullPos;
    }

    public void ReleaseString()
    {
        bowSettings.bowString.transform.position = bowSettings.stringInitialPos.position;
        bowSettings.bowString.transform.parent = bowSettings.stringInitialParent;
    }

    public void EquipBow()
    {
        this.transform.position = bowSettings.EquipPos.position;
        this.transform.rotation = bowSettings.EquipPos.rotation;
        this.transform.parent = bowSettings.EquipParent;
    }

    public void UnEquipBow()
    {
        this.transform.position = bowSettings.UnEquipPos.position;
        this.transform.rotation = bowSettings.UnEquipPos.rotation;
        this.transform.parent = bowSettings.UnEquipParent;
    }

    public void ShowCrosshair(Vector3 crosshairPos)
    {
        if (!currentCrossHair)
            currentCrossHair = Instantiate(crossHairPrefab);

        currentCrossHair.transform.position = crosshairPos;
        currentCrossHair.transform.LookAt(Camera.main.transform);
    }

    public void RemoveCrosshair()
    {
        if (currentCrossHair)
            Destroy(currentCrossHair);
    }

    public void PullAudio()
    {
        bowAudio.PlayOneShot(bowSettings.pullStringAudio);
    }

    // public void Fire(Vector3 hitPoint)
    // {
    //     if (bowSettings.arrowCount < 1)
    //         return;

    //     bowAudio.PlayOneShot(bowSettings.releaseStringAudio);
    //     Vector3 dir = hitPoint - bowSettings.arrowPos.position;
    //     currentArrow = Instantiate(bowSettings.arrowPrefab, bowSettings.arrowPos.position, bowSettings.arrowPos.rotation) as Rigidbody;

    //     currentArrow.AddForce(dir * bowSettings.arrowForce, ForceMode.Force);

    //     bowSettings.arrowCount -= 1;
    // }
    public void Fire(Vector3 targetPoint)
    {
        Vector3 start = transform.position + transform.forward * 0.5f; // Start position slightly in front of the bow
        Vector3 target = targetPoint;

        Vector3 velocity;
        bool success = CalculateBallisticVelocity(start, target, bowSettings.arrowSpeed, out velocity);

        Rigidbody arrow = Instantiate(bowSettings.arrowPrefab, start, Quaternion.LookRotation(velocity));
        arrow.linearVelocity = success ? velocity : (target - start).normalized * bowSettings.arrowSpeed;
    }
    
    // Solves for a parabolic trajectory from start to target given initial speed
    bool CalculateBallisticVelocity(Vector3 start, Vector3 target, float speed, out Vector3 velocity)
    {
    Vector3 toTarget = target - start;
    Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
    float y = toTarget.y;
    float xz = toTargetXZ.magnitude;

    float gravity = Physics.gravity.y * -1; // Usually -9.81

    float speedSquared = speed * speed;
    float discriminant = speedSquared * speedSquared - gravity * (gravity * xz * xz + 2 * y * speedSquared);

    if (discriminant < 0)
    {
        // No solution
        velocity = Vector3.zero;
        return false;
    }

    float root = Mathf.Sqrt(discriminant);
    float highAngle = Mathf.Atan2(speedSquared + root, gravity * xz); // You can use high or low angle
    float angle = highAngle;

    Vector3 directionXZ = toTargetXZ.normalized;
    velocity = directionXZ * Mathf.Cos(angle) * speed + Vector3.up * Mathf.Sin(angle) * speed;
    return true;
}
}
