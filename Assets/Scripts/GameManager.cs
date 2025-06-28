using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameData gameData;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString("GameData", json);
        PlayerPrefs.Save();
    }
    
    public void LoadGameData()
    {
        if (PlayerPrefs.HasKey("GameData"))
        {
            string json = PlayerPrefs.GetString("GameData");
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData();
        }
    }
    
    public bool CanAfford(int price)
    {
        return gameData.coins >= price;
    }
    
    public bool BuyWeapon(WeaponType weapon, int price)
    {
        if (CanAfford(price) && !gameData.ownedWeapons.Contains(weapon))
        {
            gameData.coins -= price;
            gameData.ownedWeapons.Add(weapon);
            SaveGameData();
            return true;
        }
        return false;
    }
    
    public void EquipWeapon(WeaponType weapon)
    {
        if (gameData.ownedWeapons.Contains(weapon))
        {
            gameData.currentEquippedWeapon = weapon;
            SaveGameData();
        }
    }
    
    public void AddCoins(int amount)
    {
        gameData.coins += amount;
        SaveGameData();
    }
}