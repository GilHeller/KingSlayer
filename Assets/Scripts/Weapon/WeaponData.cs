using UnityEngine;
public enum WeaponType
{
    None,
    Knife,
    Sword,
    Shield,
    Bow,
    Mana
}

[System.Serializable]
[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
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
    public Sprite icon;
}

