using UnityEngine;

using UnityEngine.UI;
using TMPro;


public class WeaponStoreUI : MonoBehaviour
{
    [Header("Weapon Info Panel")]
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponPowerText;
    public TextMeshProUGUI weaponPriceText;
    public Image weaponPreviewImage;
    
    [Header("Player Info")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI lifeText;
    
    [Header("Buy Button")]
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;
    
    [Header("Buy Button States")]
    public Color buyButtonNormalColor = Color.green;
    public Color buyButtonDisabledColor = Color.gray;
    public string buyButtonNormalText = "Buy";
    public string buyButtonOwnedText = "OWNED";
    public string buyButtonNoMoneyText = "NOT ENOUGH MONEY";
    
    private WeaponStoreManager storeManager;
    
    private void Start()
    {
        // Initialize the UI elements
        storeManager = FindFirstObjectByType<WeaponStoreManager>();
        if (storeManager == null)
        {
            Debug.LogError("WeaponStoreManager not found in the scene.");
            return;
        }
        
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(() => storeManager.BuySelectedWeapon());
        }
    }

    public void UpdateWeaponInfo(WeaponData weapon)
    {
        if (weaponNameText != null)
            weaponNameText.text = weapon.weaponName.Replace(" [Paint]", "");

        if (weaponPowerText != null) {
            if (weapon.weaponType == WeaponType.Shield)
            {
                weaponPowerText.text = $"Shield: {Mathf.Abs(weapon.damage)}";
            }
            else if (weapon.weaponType == WeaponType.Bow)
            {
                weaponPowerText.text = $"Range: {weapon.range:F1}m, Speed: {weapon.attackSpeed:F2}s";
            }
            else if (weapon.weaponType == WeaponType.Mana)
            {
                weaponPowerText.text = $"Mana Upgrade: {weapon.damage}";
            }
    }
        
        if (weaponPriceText != null)
            weaponPriceText.text = $"${weapon.price}";
        
        if (weaponPreviewImage != null && weapon.icon != null)
            weaponPreviewImage.sprite = weapon.icon;
    }
    
    public void UpdateMoneyDisplay(int money)
    {
        if (moneyText != null)
            moneyText.text = $"Money: ${money}";
    }

    public void UpdateLifeDisplay(int life)
    {
        if (lifeText != null)
            lifeText.text = $"Health: {life}";
            
            switch (life)
            {
                case > 75:
                    lifeText.color = Color.green;
                    break;
                case > 25 and <= 75:
                    lifeText.color = Color.yellow;
                    break;
                default:
                    lifeText.color = Color.red;
                    break;
            }
    }
    
    public void SetBuyButtonState(bool canBuy)
    {
        if (buyButton == null) return;

        buyButton.interactable = canBuy;

        if (buyButtonText != null)
        {
            if (canBuy)
            {
                buyButtonText.text = buyButtonNormalText;
                buyButtonText.color = buyButtonNormalColor;
            }
            else
            {
                // Determine why we can't buy
                if (storeManager.selectedIcon != null)
                {
                    WeaponData selectedWeapon = storeManager.selectedIcon.WeaponData;
                    if (storeManager.IsWeaponOwnedByPlayer(selectedWeapon.weaponType) && selectedWeapon.weaponType != WeaponType.Mana)
                    {
                        buyButtonText.text = buyButtonOwnedText;
                    }
                    else if (GameManager.Instance.CanAfford(selectedWeapon.price))
                    {
                        buyButtonText.text = buyButtonNormalText;
                    }
                    else
                    {
                        buyButtonText.text = buyButtonNoMoneyText;
                    }
                }
                buyButtonText.color = buyButtonDisabledColor;
            }
        }
    }
}



// public class WeaponStoreUI : MonoBehaviour
// {
//     [Header("UI Elements")]
//     public TextMeshProUGUI coinsText;
//     public GameObject weaponInfoPanel;
//     public TextMeshProUGUI weaponNameText;
//     public TextMeshProUGUI weaponStatsText;
//     public TextMeshProUGUI weaponPriceText;
//     public Button buyButton;
//     public Button equipButton;
//     public Button closeButton;
    
//     private WeaponData currentWeaponData;
    
//     void Start()
//     {
//         UpdateUI();
//         weaponInfoPanel.SetActive(false);
        
//         // Setup button events
//         if (buyButton != null) buyButton.onClick.AddListener(BuyCurrentWeapon);
//         if (equipButton != null) equipButton.onClick.AddListener(EquipCurrentWeapon);
//         if (closeButton != null) closeButton.onClick.AddListener(CloseWeaponInfo);
//     }
    
//     void Update()
//     {
//         UpdateCoinsDisplay();
//     }
    
//     public void UpdateUI()
//     {
//         UpdateCoinsDisplay();
//     }
    
//     void UpdateCoinsDisplay()
//     {
//         if (coinsText != null && GameManager.Instance != null)
//         {
//             coinsText.text = "Coins: " + GameManager.Instance.gameData.coins.ToString();
//         }
//     }
    
//     public void ShowWeaponDetails(WeaponData weaponData)
//     {
//         currentWeaponData = weaponData;
//         weaponInfoPanel.SetActive(true);
        
//         if (weaponNameText != null)
//             weaponNameText.text = weaponData.weaponName;
        
//         if (weaponStatsText != null)
//         {
//             weaponStatsText.text = $"Damage: {weaponData.damage}\nRange: {weaponData.range}\nSpeed: {weaponData.attackSpeed}";
//         }
        
//         if (weaponPriceText != null)
//             weaponPriceText.text = "Price: " + weaponData.price.ToString();
        
//         // Update button states
//         bool isOwned = GameManager.Instance.gameData.ownedWeapons.Contains(weaponData.weaponType);
//         bool canAfford = GameManager.Instance.CanAfford(weaponData.price);
        
//         if (buyButton != null)
//         {
//             buyButton.interactable = !isOwned && canAfford;
//             buyButton.GetComponentInChildren<TextMeshProUGUI>().text = isOwned ? "OWNED" : "BUY";
//         }
        
//         if (equipButton != null)
//         {
//             equipButton.interactable = isOwned;
//         }
//     }
    
//     void BuyCurrentWeapon()
//     {
//         if (currentWeaponData != null)
//         {
//             WeaponStore store = FindObjectOfType<WeaponStore>();
//             if (store != null)
//             {
//                 store.BuyWeapon(currentWeaponData.weaponType);
//             }
//         }
//     }
    
//     void EquipCurrentWeapon()
//     {
//         if (currentWeaponData != null)
//         {
//             WeaponManager weaponManager = FindObjectOfType<WeaponManager>();
//             if (weaponManager != null)
//             {
//                 weaponManager.EquipWeapon(currentWeaponData.weaponType);
//             }
//         }
//     }
    
//     void CloseWeaponInfo()
//     {
//         weaponInfoPanel.SetActive(false);
//     }
// }