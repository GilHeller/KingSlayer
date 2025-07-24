// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using UnityEditor;

// [System.Serializable]
// public class StoreItem
// {
//     public WeaponType weaponType;
//     public GameObject displayModel;
//     public Transform displayPosition;
// }

// public static class Extensions
// {
//     public static List<T> Shuffle<T>(this List<T> source)
//     {
//         source.Sort((i, j) => Random.Range(0, 3) - 1);

//         return source;
//     }
// }

// public class WeaponStore : MonoBehaviour
// {
//     [Header("Store Setup")]
//     // public StoreItem[] storeItems;
//     public WeaponStoreUI storeUI;

//     [Header("Store Inventory")]
//     public WeaponData[] availableWeapons;

//     [Header("Icon Display")]
//     public GameObject Icon;
//     public Transform Grid;

//     // public List<Object> Folders;
//     private string iconsFolder = "Assets/images/Icons";
//     private string outputFolder = "Assets/GeneratedWeapons";


//     void Start()
//     {
//         // SetupDisplayWeapons();
//         SetupIconGrid();
//     }

//     // void SetupDisplayWeapons()
//     // {

//     //     foreach (StoreItem item in availableWeapons)
//     //     {
//     //         if (item.displayModel != null && item.displayPosition != null)
//     //         {
//     //             GameObject display = Instantiate(item.displayModel, item.displayPosition.position, item.displayPosition.rotation);

//     //             // Add click detection
//     //             WeaponDisplay weaponDisplay = display.AddComponent<WeaponDisplay>();
//     //             weaponDisplay.weaponType = item.weaponType;
//     //             weaponDisplay.weaponStore = this;
//     //         }
//     //     }
//     // }

//     void SetupIconGrid()
//     {
//         if (!Directory.Exists(iconsFolder))
//         {
//             Debug.LogError("Icons folder not found.");
//             return;
//         }

//         if (!AssetDatabase.IsValidFolder(outputFolder))
//         {
//             AssetDatabase.CreateFolder("Assets", "GeneratedWeapons");
//         }

//         var weaponTypes = System.Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().Where(w => w != WeaponType.None);
//         int count = 0;

//         var subfolders = Directory.GetDirectories(iconsFolder);
//         Debug.Log($"Found {subfolders.Length} subfolders in {iconsFolder}");
//         foreach (var folder in subfolders)
//         {
//             Debug.Log($"Processing folder: {folder}");
//             string folderName = Path.GetFileName(folder);
//             WeaponType weaponType = MapFolderToWeaponType(folderName);
//             Debug.Log($"Mapped folder '{folderName}' to weapon type: {weaponType}");
//             if (weaponType == WeaponType.None) continue;

//             string weaponFolder = Path.Combine(iconsFolder, weaponType.ToString());
//             if (!Directory.Exists(weaponFolder)) continue;

//             string[] images = Directory.GetFiles(weaponFolder, "*.png");
//             Debug.Log($"Found {images.Length} images in folder '{weaponFolder}' for weapon type: {weaponType}");

//             foreach (var imagePath in images)
//             {
//                 Debug.Log($"Processing image: {imagePath}");
//                 string unityPath = imagePath.Replace(Application.dataPath, "Assets").Replace("\\", "/");
//                 Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(unityPath);
//                 if (icon == null) continue;

//                 // Create WeaponData ScriptableObject
//                 WeaponData weapon = ScriptableObject.CreateInstance<WeaponData>();
//                 string fileName = Path.GetFileNameWithoutExtension(imagePath);
//                 weapon.weaponType = weaponType;
//                 weapon.weaponName = fileName;
//                 weapon.icon = icon;

//                 availableWeapons = availableWeapons.Append(weapon).ToArray();

//                 // Randomized per weapon type
//                 ApplyRandomStats(weapon, weaponType);

//                 // string savePath = $"{outputFolder}/{weapon.weaponName}_{weaponType}.asset";
//                 // AssetDatabase.CreateAsset(weapon, savePath);
//                 // EditorUtility.SetDirty(weapon);
//                 count++;
//             }
//         }

//         // AssetDatabase.SaveAssets();
//         // AssetDatabase.Refresh();
//         // Debug.Log($"Generated {count} weapons in {outputFolder}");

//         // availableWeapons.Shuffle();

//         // foreach (Transform child in Grid)
//         // {
//         //     Destroy(child.gameObject);
//         // }

//         foreach (WeaponData weaponData in availableWeapons)
//         {
//             Debug.Log($"Adding weapon: {weaponData.weaponName} of type {weaponData.weaponType}");
//             var node = Instantiate(Icon, Grid);

//             node.GetComponentsInChildren<Image>(true)[1].sprite = weaponData.icon;
//             node.gameObject.SetActive(true);
//         }
//         Debug.Log($"Added {availableWeapons.Length} weapons to the icon grid.");

//         //     foreach (var weaponData in availableWeapons){
//         //         if (weaponData.icon == null) continue;

//         //         GameObject iconGO = Instantiate(iconPrefab, iconGridParent);
//         //         Image iconImage = iconGO.GetComponentInChildren<Image>();
//         //         if (iconImage != null)
//         //         {
//         //             iconImage.sprite = weaponData.icon;
//         //         }

//         //         Button button = iconGO.GetComponent<Button>();
//         //         if (button != null)
//         //         {
//         //             WeaponType type = weaponData.weaponType;
//         //             button.onClick.AddListener(() => ShowWeaponInfo(type));
//         //         }

//         //         iconGO.SetActive(true);
//         // }
//     }

//         private void ApplyRandomStats(WeaponData weapon, WeaponType type){
//         switch (type)
//         {
//             case WeaponType.Knife:
//                 weapon.damage = Random.Range(5, 10);
//                 weapon.range = Random.Range(0.5f, 1f);
//                 weapon.attackSpeed = Random.Range(1.2f, 2f);
//                 weapon.price = Random.Range(50, 100);
//                 weapon.isRanged = false;
//                 break;
//             case WeaponType.Sword:
//                 weapon.damage = Random.Range(10, 20);
//                 weapon.range = Random.Range(1.5f, 2.5f);
//                 weapon.attackSpeed = Random.Range(0.8f, 1.5f);
//                 weapon.price = Random.Range(150, 250);
//                 weapon.isRanged = false;
//                 break;
//             case WeaponType.Shield:
//                 weapon.damage = Random.Range(2, 5);
//                 weapon.range = Random.Range(1f, 1.2f);
//                 weapon.attackSpeed = Random.Range(0.5f, 1f);
//                 weapon.price = Random.Range(120, 180);
//                 weapon.isRanged = false;
//                 break;
//             case WeaponType.Bow:
//                 weapon.damage = Random.Range(15, 25);
//                 weapon.range = Random.Range(4f, 6f);
//                 weapon.attackSpeed = Random.Range(0.7f, 1.2f);
//                 weapon.price = Random.Range(200, 300);
//                 weapon.isRanged = true;
//                 break;
//         }

//         weapon.weaponModel = null;         // Assign manually or extend
//         weapon.attackAnimation = null;     // Assign manually or extend
//     }

//     private WeaponType MapFolderToWeaponType(string folderName)
//     {
//         switch (folderName.ToLower())
//         {
//             case "sword": return WeaponType.Sword;
//             case "knife": return WeaponType.Knife;
//             case "bow":
//             case "arrow":
//                 return WeaponType.Bow;
//             case "shield":
//             case "helmet":
//             case "armor":
//             case "boots":
//                 return WeaponType.Shield;
//             default:
//                 return WeaponType.None; // or throw/log
//         }
//     }


//     public void ShowWeaponInfo(WeaponType weaponType)
//     {
//         WeaponData weaponData = System.Array.Find(availableWeapons, w => w.weaponType == weaponType);
//         if (weaponData != null && storeUI != null)
//         {
//             storeUI.ShowWeaponDetails(weaponData);
//         }
//     }

//     public void BuyWeapon(WeaponType weaponType)
//     {
//         WeaponData weaponData = System.Array.Find(availableWeapons, w => w.weaponType == weaponType);
//         if (weaponData != null)
//         {
//             if (GameManager.Instance.BuyWeapon(weaponType, weaponData.price))
//             {
//                 Debug.Log($"Purchased {weaponData.weaponName}!");
//                 if (storeUI != null)
//                 {
//                     storeUI.UpdateUI();
//                 }
//             }
//             else
//             {
//                 Debug.Log("Not enough coins or already owned!");
//             }
//         }
//     }
// }