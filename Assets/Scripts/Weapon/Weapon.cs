using UnityEngine;

public enum WeaponType
{
    None,
    Knife,
    Sword,
    Shield,
    Bow
}

[System.Serializable]
public class WeaponData
{
    public WeaponType weaponType;
    public string weaponName;
    public int damage;
    public float range;
    public float attackSpeed;
    public int price;
    public GameObject weaponModel;
    public Animator attackAnimation;
    public bool isRanged;
}

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