using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int coins = 0; // Starting coins
    public int health = 100; // Starting health
    public List<WeaponType> ownedWeapons = new List<WeaponType>();
    // public WeaponType currentEquippedWeapon = WeaponType.None;
    public WeaponData currentEquippedWeapon;

    public GameObject activePlayer;

    public Vector3 spawnPoint;
    public bool isNewScene = false;

    public static bool isGameOver = false;

    public void Init()
    {
        // General initialization for currentEquippedWeapon
        currentEquippedWeapon = ScriptableObject.CreateInstance<WeaponData>();
        activePlayer = GameObject.FindGameObjectWithTag("Player") ?? null;
        spawnPoint = Vector3.zero; // Default spawn point   
    }
    
    void Update()
    {
        // Update the active player reference if it changes
        if (activePlayer == null)
        {
            Debug.LogWarning("Active player not found, trying to find by tag.");
            activePlayer = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("Active player updated: " + (activePlayer != null ? activePlayer.name : "null"));
        }
    }
}
