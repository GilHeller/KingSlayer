using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Setup")]
    public WeaponData[] allWeapons;
    public Transform weaponAttachPoint;
    public Animator playerAnimator;

    public GameObject playerToSwitchPrefab;
    
    private Weapon currentWeapon;
    private GameObject currentWeaponModel;
    
    void Start()
    {
        // Equip the currently equipped weapon from save data
        if (GameManager.Instance != null)
        {

            EquipWeapon(GameManager.Instance.gameData.currentEquippedWeapon.weaponType, false);
        }
    }
    
    void Update()
    {
        HandleInput();
    }

    public void SwitchPrefab()
    {
        Debug.Log("Switching to archer mode and replacing player model");


        // Save current position & rotation
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        // Instantiate archer prefab
        Debug.Log("Instantiating archer prefab at position: " + position + ", rotation: " + rotation);
        Transform parentTransform = transform.parent;
        GameObject newPlayer = Instantiate(playerToSwitchPrefab, position, rotation, parentTransform);
        Debug.Log("Archer prefab instantiated: " + newPlayer.name);

        // // Destroy current player
        // Debug.Log("Destroying current player: " + currentPlayer.name);
        Debug.Log("Current player destroyed");
        Destroy(gameObject);
        // System.Threading.Thread.Sleep(2000); // Wait for the new player to initialize
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
            // Reassign camera target to new player
            cameraController.FindPlayer();
            // cameraController.FollowPlayer();
        }

        // if (newPlayer.name.Contains("Archer"))
        // {
        //     System.Threading.Thread.Sleep(2000); // Wait for the new player to initialize
        //     newPlayer.GetComponent<InputSystem>().enabled = true;
        // }

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
    
    public void EquipWeapon(WeaponType weaponType, bool switchPrefab = true)
    {
        Debug.Log($"Equipping weapon: {weaponType}");

        if (weaponType == WeaponType.Bow && switchPrefab)
        {
            Debug.Log("Switching to archer mode");
            SwitchPrefab();
            return;
        }
        // Find weapon data
        WeaponData weaponData = System.Array.Find(allWeapons, w => w.weaponType == weaponType);
        if (weaponData == null) return;
        
        // Destroy current weapon
        if (currentWeaponModel != null)
        {
            DestroyImmediate(currentWeaponModel);
        }
        
        // Create new weapon
        // if (weaponData.weaponModel != null)
        // {
        //     currentWeaponModel = Instantiate(weaponData.weaponModel, weaponAttachPoint);
        //     currentWeaponModel.transform.localPosition = Vector3.zero;
        //     currentWeaponModel.transform.localRotation = Quaternion.identity;

        //     // Get weapon component
        //     currentWeapon = currentWeaponModel.GetComponent<Weapon>();
        //     if (currentWeapon == null)
        //     {
        //         // Add appropriate weapon component based on type
        //         switch (weaponType)
        //         {
        //             case WeaponType.Knife:
        //                 // currentWeapon = currentWeaponModel.AddComponent<Knife>();
        //                 break;
        //             case WeaponType.Sword:
        //                 // currentWeapon = currentWeaponModel.AddComponent<Sword>();
        //                 break;
        //             case WeaponType.Shield:
        //                 // currentWeapon = currentWeaponModel.AddComponent<Shield>();
        //                 break;
        //             case WeaponType.Bow:
        //                 // currentWeapon = currentWeaponModel.AddComponent<Bow>();
        //                 SwitchToArcher();
        //                 break;
        //         }

        //         if (currentWeapon != null)
        //         {
        //             currentWeapon.weaponData = weaponData;
        //         }
        //     }
        // }

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