using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEditor;


public class WeaponStoreManager : MonoBehaviour
{
    [Header("Store Setup")]
    public WeaponStoreUI storeUI;
    
    [Header("Dynamic Generation")]
    public bool useDynamicGeneration = true;
    private string iconsFolder = "Assets/images/Icons";
    
    [Header("Manual Setup (if not using dynamic)")]
    public WeaponData[] availableWeapons;
    
    [Header("Icon Display")]
    public Transform grid;
    
    [Header("Icon Prefab Settings")]
    public Vector2 iconSize = new Vector2(80, 80);
    public Sprite  normalColor;
    public Sprite  selectedColor;
    public Sprite  ownedColor;
    
    private List<WeaponIconController> weaponIcons = new List<WeaponIconController>();
    public WeaponIconController selectedIcon;
    
    private void Start()
    {
        if (useDynamicGeneration)
        {
            LoadWeaponDataFromFolder();
        }
        
        GenerateWeaponIcons();
        UpdateMoneyDisplay();
    }

    private void LoadWeaponDataFromFolder()
    {
        // // Load all WeaponData ScriptableObjects from the folder
        // WeaponData[] weaponDataArray = Resources.LoadAll<WeaponData>(weaponDataFolder);

        // if (weaponDataArray.Length > 0)
        // {
        //     availableWeapons = weaponDataArray;
        //     Debug.Log($"Loaded {availableWeapons.Length} weapons from folder: {weaponDataFolder}");
        // }
        // else
        // {
        //     Debug.LogWarning($"No WeaponData found in Resources/{weaponDataFolder}");
        // }

        if (!Directory.Exists(iconsFolder))
        {
            Debug.LogError("Icons folder not found.");
            return;
        }

        Debug.Log($"Loading weapon icons from folder: {iconsFolder}");
        availableWeapons = new WeaponData[0]; // Initialize empty array

        var weaponTypes = System.Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().Where(w => w != WeaponType.None);
        int count = 0;

        var subfolders = Directory.GetDirectories(iconsFolder);
        Debug.Log($"Found {subfolders.Length} subfolders in {iconsFolder}");
        foreach (var folder in subfolders)
        {
            Debug.Log($"Processing folder: {folder}");
            string folderName = Path.GetFileName(folder);
            WeaponType weaponType = MapFolderToWeaponType(folderName);
            Debug.Log($"Mapped folder '{folderName}' to weapon type: {weaponType}");
            if (weaponType == WeaponType.None) continue;

            string weaponFolder = Path.Combine(iconsFolder, weaponType.ToString());
            if (!Directory.Exists(weaponFolder)) continue;

            string[] images = Directory.GetFiles(weaponFolder, "*.png");
            Debug.Log($"Found {images.Length} images in folder '{weaponFolder}' for weapon type: {weaponType}");

            foreach (var imagePath in images)
            {
                Debug.Log($"Processing image: {imagePath}");
                string unityPath = imagePath.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(unityPath);
                if (icon == null) continue;

                // Create WeaponData ScriptableObject
                WeaponData weapon = ScriptableObject.CreateInstance<WeaponData>();
                string fileName = Path.GetFileNameWithoutExtension(imagePath);
                weapon.weaponType = weaponType;
                weapon.weaponName = fileName;
                weapon.icon = icon;

                availableWeapons = availableWeapons.Append(weapon).ToArray();

                // Randomized per weapon type
                ApplyRandomStats(weapon, weaponType);

                // string savePath = $"{outputFolder}/{weapon.weaponName}_{weaponType}.asset";
                // AssetDatabase.CreateAsset(weapon, savePath);
                // EditorUtility.SetDirty(weapon);
                count++;
            }
        }
    }

            private void ApplyRandomStats(WeaponData weapon, WeaponType type){
        switch (type)
        {
            case WeaponType.Knife:
                weapon.damage = Random.Range(5, 10);
                weapon.range = Random.Range(0.5f, 1f);
                weapon.attackSpeed = Random.Range(1.2f, 2f);
                weapon.price = Random.Range(50, 100);
                weapon.isRanged = false;
                break;
            case WeaponType.Sword:
                weapon.damage = Random.Range(10, 20);
                weapon.range = Random.Range(1.5f, 2.5f);
                weapon.attackSpeed = Random.Range(0.8f, 1.5f);
                weapon.price = Random.Range(150, 250);
                weapon.isRanged = false;
                break;
            case WeaponType.Shield:
                weapon.damage = Random.Range(2, 5);
                weapon.range = Random.Range(1f, 1.2f);
                weapon.attackSpeed = Random.Range(0.5f, 1f);
                weapon.price = Random.Range(120, 180);
                weapon.isRanged = false;
                break;
            case WeaponType.Bow:
                weapon.damage = Random.Range(15, 25);
                weapon.range = Random.Range(4f, 6f);
                weapon.attackSpeed = Random.Range(0.7f, 1.2f);
                weapon.price = Random.Range(200, 300);
                weapon.isRanged = true;
                break;
        }

        weapon.weaponModel = null;         // Assign manually or extend
        weapon.attackAnimation = null;     // Assign manually or extend
    }

    private WeaponType MapFolderToWeaponType(string folderName)
    {
        switch (folderName.ToLower())
        {
            case "sword": return WeaponType.Sword;
            case "knife": return WeaponType.Knife;
            case "bow":
            case "arrow":
                return WeaponType.Bow;
            case "shield":
            case "helmet":
            case "armor":
            case "boots":
                return WeaponType.Shield;
            default:
                return WeaponType.None; // or throw/log
        }
    }

    private void GenerateWeaponIcons()
    {
        // Clear existing icons
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
        weaponIcons.Clear();

        // Generate icons for each weapon
        foreach (WeaponData weapon in availableWeapons)
        {
            GameObject iconObj = CreateWeaponIcon(weapon);
            WeaponIconController iconController = iconObj.GetComponent<WeaponIconController>();

            if (iconController != null)
            {
                iconController.Initialize(weapon, this);
                weaponIcons.Add(iconController);
            }
        }
        
        Debug.Log($"Generated {weaponIcons.Count} weapon icons.");
    }
    
    private GameObject CreateWeaponIcon(WeaponData weapon)
    {
        // Create main icon GameObject
        GameObject iconObj = new GameObject($"WeaponIcon_{weapon.weaponType}");
        iconObj.transform.SetParent(grid);
        
        // Add RectTransform
        RectTransform rectTransform = iconObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = iconSize;
        rectTransform.localScale = Vector3.one;
        
        // Add Button component
        Button button = iconObj.AddComponent<Button>();
        button.transition = Selectable.Transition.None;
        
        // Create background image
        GameObject backgroundObj = new GameObject("Background");
        backgroundObj.transform.SetParent(iconObj.transform);
        RectTransform bgRect = backgroundObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.localScale = Vector3.one;
        
        Image backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.sprite = normalColor;
        
        // Create weapon image
        GameObject weaponImageObj = new GameObject("WeaponImage");
        weaponImageObj.transform.SetParent(iconObj.transform);
        RectTransform weaponRect = weaponImageObj.AddComponent<RectTransform>();
        weaponRect.anchorMin = Vector2.zero;
        weaponRect.anchorMax = Vector2.one;
        weaponRect.sizeDelta = Vector2.zero;
        weaponRect.anchoredPosition = Vector2.zero;
        weaponRect.localScale = Vector3.one;
        
        Image weaponImage = weaponImageObj.AddComponent<Image>();
        weaponImage.sprite = weapon.icon;
        weaponImage.preserveAspect = true;
        
        // Create owned indicator
        GameObject ownedIndicatorObj = new GameObject("OwnedIndicator");
        ownedIndicatorObj.transform.SetParent(iconObj.transform);
        RectTransform ownedRect = ownedIndicatorObj.AddComponent<RectTransform>();
        ownedRect.anchorMin = new Vector2(1, 1);
        ownedRect.anchorMax = new Vector2(1, 1);
        ownedRect.sizeDelta = new Vector2(20, 20);
        ownedRect.anchoredPosition = new Vector2(-10, -10);
        ownedRect.localScale = Vector3.one;
        
        Image ownedIndicator = ownedIndicatorObj.AddComponent<Image>();
        ownedIndicator.sprite = ownedColor;
        ownedIndicator.sprite = CreateCircleSprite(); // Create a simple circle sprite
        ownedIndicatorObj.SetActive(false);
        
        // Add WeaponIconController component
        WeaponIconController iconController = iconObj.AddComponent<WeaponIconController>();
        iconController.weaponImage = weaponImage;
        iconController.backgroundImage = backgroundImage;
        iconController.ownedIndicator = ownedIndicator;
        iconController.normalColor = normalColor;
        iconController.selectedColor = selectedColor;
        iconController.ownedColor = ownedColor;
        
        return iconObj;
    }
    
    private Sprite CreateCircleSprite()
    {
        // Create a simple circle texture for owned indicator
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        
        Vector2 center = new Vector2(16, 16);
        float radius = 14f;
        
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                colors[y * 32 + x] = distance <= radius ? Color.white : Color.clear;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
    
    public void OnWeaponIconClicked(WeaponIconController clickedIcon)
    {
        // Deselect previous icon

        Debug.Log($"Weapon icon clicked: {clickedIcon.weaponData.weaponName}"); 
        if (selectedIcon != null)
        {
            selectedIcon.SetSelected(false);
        }
        
        // Select new icon
        selectedIcon = clickedIcon;
        selectedIcon.SetSelected(true);
        
        // Update UI with weapon info
        storeUI.UpdateWeaponInfo(selectedIcon.weaponData);
        
        // Update buy button state
        bool canBuy = CanBuyWeapon(selectedIcon.weaponData);
        storeUI.SetBuyButtonState(canBuy);
    }
    
    public void BuySelectedWeapon()
    {
        if (selectedIcon == null) return;
        
        WeaponData weaponToBuy = selectedIcon.weaponData;
        
        if (GameManager.Instance.BuyWeapon(weaponToBuy.weaponType, weaponToBuy.price))
        {
            // Update UI
            UpdateMoneyDisplay();
            selectedIcon.SetOwned(true);
            
            // Update buy button state
            storeUI.SetBuyButtonState(false);

            GameManager.Instance.EquipWeapon(weaponToBuy.weaponType);
            
            Debug.Log($"Purchased {weaponToBuy.weaponName} for {weaponToBuy.price} coins!");
        }
        else
        {
            Debug.Log("Cannot buy weapon - not enough coins or already owned!");
        }
    }
    
    private bool CanBuyWeapon(WeaponData weapon)
    {
        return GameManager.Instance.CanAfford(weapon.price) && !IsWeaponOwned(weapon.weaponType);
    }
    
    private bool IsWeaponOwned(WeaponType weaponType)
    {
        Debug.Log($"Checking if weapon {weaponType} is owned by player.");
        Debug.Log($"Owned weapons: {string.Join(", ", GameManager.Instance.gameData.ownedWeapons)}");
        return GameManager.Instance.gameData.ownedWeapons.Contains(weaponType);
    }
    
    private void UpdateMoneyDisplay()
    {
        storeUI.UpdateMoneyDisplay(GameManager.Instance.gameData.coins);
    }
    
    public bool IsWeaponOwnedByPlayer(WeaponType weaponType)
    {
        return IsWeaponOwned(weaponType);
    }
}