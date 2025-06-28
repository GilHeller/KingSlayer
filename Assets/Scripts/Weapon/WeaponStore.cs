using UnityEngine;

[System.Serializable]
public class StoreItem
{
    public WeaponType weaponType;
    public GameObject displayModel;
    public Transform displayPosition;
}

public class WeaponStore : MonoBehaviour
{
    [Header("Store Setup")]
    public StoreItem[] storeItems;
    public Transform shopkeeper;
    public WeaponStoreUI storeUI;
    
    [Header("Store Inventory")]
    public WeaponData[] availableWeapons;
    
    void Start()
    {
        SetupDisplayWeapons();
    }
    
    void SetupDisplayWeapons()
    {
        foreach (StoreItem item in storeItems)
        {
            if (item.displayModel != null && item.displayPosition != null)
            {
                GameObject display = Instantiate(item.displayModel, item.displayPosition.position, item.displayPosition.rotation);
                
                // Add click detection
                WeaponDisplay weaponDisplay = display.AddComponent<WeaponDisplay>();
                weaponDisplay.weaponType = item.weaponType;
                weaponDisplay.weaponStore = this;
            }
        }
    }
    
    public void ShowWeaponInfo(WeaponType weaponType)
    {
        WeaponData weaponData = System.Array.Find(availableWeapons, w => w.weaponType == weaponType);
        if (weaponData != null && storeUI != null)
        {
            storeUI.ShowWeaponDetails(weaponData);
        }
    }
    
    public void BuyWeapon(WeaponType weaponType)
    {
        WeaponData weaponData = System.Array.Find(availableWeapons, w => w.weaponType == weaponType);
        if (weaponData != null)
        {
            if (GameManager.Instance.BuyWeapon(weaponType, weaponData.price))
            {
                Debug.Log($"Purchased {weaponData.weaponName}!");
                if (storeUI != null)
                {
                    storeUI.UpdateUI();
                }
            }
            else
            {
                Debug.Log("Not enough coins or already owned!");
            }
        }
    }
}