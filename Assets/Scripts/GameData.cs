using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int coins = 400; // Starting coins
    public int health = 24; // Starting health
    public List<WeaponType> ownedWeapons = new List<WeaponType>();
    // public WeaponType currentEquippedWeapon = WeaponType.None;
    public  WeaponData currentEquippedWeapon;

    public static bool isGameOver = false;

    public void Init()
    {
        // General initialization for currentEquippedWeapon
        currentEquippedWeapon = ScriptableObject.CreateInstance<WeaponData>();
    }
}
