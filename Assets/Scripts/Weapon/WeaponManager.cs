using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Setup")]
    public WeaponData[] allWeapons;
    public Transform weaponAttachPoint;
    public Animator playerAnimator;

    public GameObject archerPrefab;
    
    private Weapon currentWeapon;
    private GameObject currentWeaponModel;
    
    void Start()
    {
        // Equip the currently equipped weapon from save data
        if (GameManager.Instance != null)
        {
            EquipWeapon(GameManager.Instance.gameData.currentEquippedWeapon.weaponType);
        }
    }
    
    void Update()
    {
        HandleInput();
    }

public void SwitchToArcher()
{
    GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
    if (currentPlayer == null)
        {
            Debug.LogError("No current player to replace!");
            return;
        }

    // Save current position & rotation
    Vector3 position = currentPlayer.transform.position;
    Quaternion rotation = currentPlayer.transform.rotation;

    // Destroy current player
    Destroy(currentPlayer);

    // Instantiate archer prefab
    currentPlayer = Instantiate(archerPrefab, position, rotation);
}
    
    void HandleInput()
    {
        if (currentWeapon != null)
        {
            // // Attack input
            // if (Input.GetMouseButtonDown(0))
            // {
            //     if (currentWeapon.weaponData.weaponType == WeaponType.Bow)
            //     {
            //         currentWeapon.StartAiming();
            //     }
            //     else
            //     {
            //         currentWeapon.Attack();
            //     }
            // }
            
            // if (Input.GetMouseButtonUp(0) && currentWeapon.weaponData.weaponType == WeaponType.Bow)
            // {
            //     currentWeapon.StopAiming();
            // }
            
            // // Shield blocking
            // if (currentWeapon.weaponData.weaponType == WeaponType.Shield)
            // {
            //     Shield shield = currentWeapon as Shield;
            //     if (Input.GetMouseButton(1))
            //     {
            //         shield.StartBlocking();
            //     }
            //     else if (Input.GetMouseButtonUp(1))
            //     {
            //         shield.StopBlocking();
            //     }
            // }
        }
        
        // Weapon switching (number keys)
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryEquipWeapon(WeaponType.Knife);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryEquipWeapon(WeaponType.Sword);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryEquipWeapon(WeaponType.Shield);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TryEquipWeapon(WeaponType.Bow);
    }
    
    void TryEquipWeapon(WeaponType weaponType)
    {
        Debug.Log($"Trying to equip weapon: {weaponType}");
        if (GameManager.Instance.gameData.ownedWeapons.Contains(weaponType))
        {
            EquipWeapon(weaponType);
        }
    }
    
    public void EquipWeapon(WeaponType weaponType)
    {
        Debug.Log($"Equipping weapon: {weaponType}");
        // Find weapon data
        WeaponData weaponData = System.Array.Find(allWeapons, w => w.weaponType == weaponType);
        if (weaponData == null) return;
        
        // Destroy current weapon
        if (currentWeaponModel != null)
        {
            DestroyImmediate(currentWeaponModel);
        }
        
        // Create new weapon
        if (weaponData.weaponModel != null)
        {
            currentWeaponModel = Instantiate(weaponData.weaponModel, weaponAttachPoint);
            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation = Quaternion.identity;
            
            // Get weapon component
            currentWeapon = currentWeaponModel.GetComponent<Weapon>();
            if (currentWeapon == null)
            {
                // Add appropriate weapon component based on type
                switch (weaponType)
                {
                    case WeaponType.Knife:
                        // currentWeapon = currentWeaponModel.AddComponent<Knife>();
                        break;
                    case WeaponType.Sword:
                        // currentWeapon = currentWeaponModel.AddComponent<Sword>();
                        break;
                    case WeaponType.Shield:
                        // currentWeapon = currentWeaponModel.AddComponent<Shield>();
                        break;
                    case WeaponType.Bow:
                        // currentWeapon = currentWeaponModel.AddComponent<Bow>();
                        SwitchToArcher();
                        break;
                }
                
                if (currentWeapon != null)
                {
                    currentWeapon.weaponData = weaponData;
                }
            }
        }
        
        // Update animator
        if (playerAnimator != null)
        {
            playerAnimator.SetInteger("WeaponType", (int)weaponType);
        }
        
        // Update save data
        GameManager.Instance.EquipWeapon(weaponType);
        
        Debug.Log($"Equipped: {weaponData.weaponName}");
    }
}