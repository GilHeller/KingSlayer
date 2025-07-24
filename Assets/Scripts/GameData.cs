using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int coins = 0; // Starting coins
    public int health = 100; // Starting health
    public List<WeaponType> ownedWeapons = new List<WeaponType>();
    // public WeaponType currentEquippedWeapon = WeaponType.None;
    public  WeaponData currentEquippedWeapon;

    public GameObject activePlayer;

    public Vector3 spawnPoint;

    public static bool isGameOver = false;

    public void Init()
    {
        // General initialization for currentEquippedWeapon
        currentEquippedWeapon = ScriptableObject.CreateInstance<WeaponData>();
        activePlayer = GameObject.FindGameObjectWithTag("Player") ?? null;
        spawnPoint = Vector3.zero; // Default spawn point   
    }
}
