using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameData gameData;

    public bool LoadFromPrevious = false;

    void Awake()
    {
        if (!LoadFromPrevious)
        {
            PlayerPrefs.DeleteAll();
        }

        // gameData.activePlayer.SetActive(true); // Ensure the active player is set to true

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

    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Play" && !this.gameData.activePlayer)
        {
            this.gameData.activePlayer = GameObject.FindGameObjectWithTag("Player");
        }

        //this.gameData.activePlayer.GetComponentInParent<GameObject>().transform.position = this.gameData.activePlayer.transform.position;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != this.gameData.activePlayer)
            {
                player.gameObject.transform.position = this.gameData.activePlayer.transform.position;
            }
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
            gameData.Init();
        }
    }
    
    public bool CanAfford(int price)
    {
        return gameData.coins >= price;
    }

    public bool IsWeaponOwned(WeaponType weaponType)
    {
        if (weaponType == WeaponType.Mana) return false;
        return gameData.ownedWeapons.Contains(weaponType);
    }
    
    public bool BuyWeapon(WeaponType weapon, int price)
    {
        if (CanAfford(price) && !IsWeaponOwned(weapon))
        {
            gameData.coins -= price;
            gameData.ownedWeapons.Add(weapon);
            SaveGameData();
            return true;
        }
        return false;
    }

    public void ApplyManaUpgrade(int mana)
    {
        gameData.health = Mathf.Min(gameData.health + mana, 100);
        SaveGameData();
    }
    
    public void EquipWeapon(WeaponType weapon)
    {
        Debug.Log($"Equipping weapon: {weapon}");
        Debug.Log(gameData.currentEquippedWeapon);
        if (gameData.ownedWeapons.Contains(weapon))
        {
            gameData.currentEquippedWeapon.weaponType = weapon;
            SaveGameData();
        }
    }
    
    public void AddCoins(int amount)
    {
        gameData.coins += amount;
        SaveGameData();
    }
}