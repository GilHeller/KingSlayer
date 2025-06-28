using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int coins = 100; // Starting coins
    public List<WeaponType> ownedWeapons = new List<WeaponType>();
    public WeaponType currentEquippedWeapon = WeaponType.None;

    public static bool isGameOver = false;
    
    public GameData()
    {
        // Start with basic knife
        ownedWeapons.Add(WeaponType.Knife);
        currentEquippedWeapon = WeaponType.Knife;
    }
}
