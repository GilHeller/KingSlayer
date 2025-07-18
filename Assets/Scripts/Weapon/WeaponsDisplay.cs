using UnityEngine;

public class WeaponDisplay : MonoBehaviour
{
    public WeaponType weaponType;
    public WeaponStoreManager weaponStore;
    
    void OnMouseDown()
    {
        if (weaponStore != null)
        {
            // weaponStore.ShowWeaponInfo(weaponType);
            Debug.Log($"Weapon {weaponType} clicked.");
        }
    }
    
    void OnMouseEnter()
    {
        // Add highlight effect
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }
    
    void OnMouseExit()
    {
        // Remove highlight effect
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }
    }
}
