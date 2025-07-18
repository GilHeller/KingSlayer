using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int coins = 400; // Starting coins
    public List<WeaponType> ownedWeapons = new List<WeaponType>();
    // public WeaponType currentEquippedWeapon = WeaponType.None;
    public  WeaponData currentEquippedWeapon;

    public static bool isGameOver = false;

    public GameData()
    {
        // Start with basic knife
        // ownedWeapons.Add(WeaponType.Knife);
        // currentEquippedWeapon = WeaponType.Knife;
        currentEquippedWeapon = null; // No weapon equipped initially
    }
}
