using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class WeaponIconController : MonoBehaviour
{
    [Header("UI References")]
    public Image weaponImage;
    public Image backgroundImage;
    public Image ownedIndicator; // Optional: shows if weapon is owned
    
    [Header("Visual States")]
    public Sprite  normalColor;
    public Sprite  selectedColor;
    public Sprite  ownedColor;
    
    public WeaponData weaponData;
    private WeaponStoreManager storeManager;
    private Button button;
    private bool isSelected;
    private bool isOwned;
    
    public WeaponData WeaponData => weaponData;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }
    
    public void Initialize(WeaponData weapon, WeaponStoreManager manager)
    {
        weaponData = weapon;
        storeManager = manager;
        
        // Set weapon icon
        if (weaponImage != null && weapon.icon != null)
        {
            weaponImage.sprite = weapon.icon;
        }

        Debug.Log($"Initializing WeaponIconController for {weapon.weaponName}");
        
        // Check if weapon is already owned
        isOwned = storeManager.IsWeaponOwnedByPlayer(weapon.weaponType);
        UpdateVisualState();
    }
    
    private void OnClick()
    {
        storeManager.OnWeaponIconClicked(this);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }
    
    public void SetOwned(bool owned)
    {
        isOwned = owned;
        UpdateVisualState();
    }
    
    private void UpdateVisualState()
    {
        if (backgroundImage != null)
        {
            if (isOwned)
            {
                backgroundImage.sprite = ownedColor;
            }
            else if (isSelected)
            {
                backgroundImage.sprite = selectedColor;
            }
            else
            {
                backgroundImage.sprite = normalColor;
            }

            float targetScale = isSelected ? 1.1f : 1f;
            transform.localScale = Vector3.one * targetScale;
        }
        
        // Update owned indicator
        if (ownedIndicator != null)
        {
            ownedIndicator.gameObject.SetActive(isOwned);
        }
    }
}