using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponStoreUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI coinsText;
    public GameObject weaponInfoPanel;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;
    public TextMeshProUGUI weaponPriceText;
    public Button buyButton;
    public Button equipButton;
    public Button closeButton;
    
    private WeaponData currentWeaponData;
    
    void Start()
    {
        UpdateUI();
        weaponInfoPanel.SetActive(false);
        
        // Setup button events
        if (buyButton != null) buyButton.onClick.AddListener(BuyCurrentWeapon);
        if (equipButton != null) equipButton.onClick.AddListener(EquipCurrentWeapon);
        if (closeButton != null) closeButton.onClick.AddListener(CloseWeaponInfo);
    }
    
    void Update()
    {
        UpdateCoinsDisplay();
    }
    
    public void UpdateUI()
    {
        UpdateCoinsDisplay();
    }
    
    void UpdateCoinsDisplay()
    {
        if (coinsText != null && GameManager.Instance != null)
        {
            coinsText.text = "Coins: " + GameManager.Instance.gameData.coins.ToString();
        }
    }
    
    public void ShowWeaponDetails(WeaponData weaponData)
    {
        currentWeaponData = weaponData;
        weaponInfoPanel.SetActive(true);
        
        if (weaponNameText != null)
            weaponNameText.text = weaponData.weaponName;
        
        if (weaponStatsText != null)
        {
            weaponStatsText.text = $"Damage: {weaponData.damage}\nRange: {weaponData.range}\nSpeed: {weaponData.attackSpeed}";
        }
        
        if (weaponPriceText != null)
            weaponPriceText.text = "Price: " + weaponData.price.ToString();
        
        // Update button states
        bool isOwned = GameManager.Instance.gameData.ownedWeapons.Contains(weaponData.weaponType);
        bool canAfford = GameManager.Instance.CanAfford(weaponData.price);
        
        if (buyButton != null)
        {
            buyButton.interactable = !isOwned && canAfford;
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = isOwned ? "OWNED" : "BUY";
        }
        
        if (equipButton != null)
        {
            equipButton.interactable = isOwned;
        }
    }
    
    void BuyCurrentWeapon()
    {
        if (currentWeaponData != null)
        {
            WeaponStore store = FindObjectOfType<WeaponStore>();
            if (store != null)
            {
                store.BuyWeapon(currentWeaponData.weaponType);
            }
        }
    }
    
    void EquipCurrentWeapon()
    {
        if (currentWeaponData != null)
        {
            WeaponManager weaponManager = FindObjectOfType<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.EquipWeapon(currentWeaponData.weaponType);
            }
        }
    }
    
    void CloseWeaponInfo()
    {
        weaponInfoPanel.SetActive(false);
    }
}
