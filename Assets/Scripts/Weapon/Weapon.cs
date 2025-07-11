using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponData weaponData;
    
    public abstract void Attack();
    public abstract void StartAiming();
    public abstract void StopAiming();
    
    protected virtual void Start()
    {
        // Weapon-specific initialization
    }
}