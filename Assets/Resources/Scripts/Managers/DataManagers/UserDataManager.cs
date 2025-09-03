using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Data.Player;
using System;
using System.IO;
using Data.Date;
using Data.Inventory;

public class UserDataManager
{
    private List<InventoryItemSave> _initialInventoryItems = new List<InventoryItemSave>
    {
        new InventoryItemSave(0, 1001, 1, 0),
        new InventoryItemSave(1, 1002, 1, 0),
        new InventoryItemSave(2, 1003, 1, 0),
        new InventoryItemSave(3, 1004, 1, 0),
        new InventoryItemSave(4, 1005, 1, 0),
    };
    private DataManager dataManager => Managers.Data;
    private Dictionary<int, PlayerData> _playerDatas = new Dictionary<int, PlayerData>();
    private IWallet _wallet;
    public IWallet WalletSerivce => _wallet ?? new PlayerWallet();
    public PlayerData CurrentData { get; private set; }

    public void Init()
    {
        LoadAllSaveData();
    }

    public void SetCurrentSaveData(PlayerData data)
    {
        if (data == null)
        {
            data = CreateNewData();
        }

        CurrentData = data;
    }

    public PlayerData CreateNewData(string playerName = "Player", string farmName = "Farm", Define.PlayerGender gender = Define.PlayerGender.Man)
    {
        string saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        Date startDate = new Date(1, Define.Season.Spring, 1);
        int startGold = 500;

        PlayerData newPlayerData = new PlayerData(playerName, farmName, saveTime, startGold, gender, startDate);
        newPlayerData.playerInventory = new PlayerInventorySave { inventorySize = 30, inventory = _initialInventoryItems };

        return newPlayerData;
    }

    public PlayerData GetSaveData(int saveNumber)
    {
        if (_playerDatas.TryGetValue(saveNumber, out PlayerData data))
        {
            return data;
        }

        return null;
    }

    public void DeleteSaveData(int saveNumber)
    {
        string path = GetSaveDataPath(saveNumber);

        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            File.Delete(path);
            _playerDatas.Remove(saveNumber);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void SaveDataToJson(int saveNumber, PlayerData data = null)
    {
        if (!dataManager.IsValidSlotIndex(saveNumber))
        {
            return;
        }

        try
        {
            PlayerData saveData = null;
            if (data != null)
            {
                saveData = data;
            }
            else
            {
                saveData = CurrentData;
                saveData.date = new Date(Managers.Time.CurrentYear, Managers.Time.CurrentSeason, Managers.Time.CurrentDay);
                dataManager.InventoryDataManager.WriteBackToPlayerData(saveData);
            }

            if (saveData == null)
            {
                Debug.Log("Data is Null");
                return;
            }

            saveData.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, dataManager.Settings);
            string path = GetSaveDataPath(saveNumber);

            File.WriteAllText(path, json);
            _playerDatas[saveNumber] = saveData;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private string GetSaveDataPath(int saveNumber)
    {
        string dirPath = dataManager.GetDirectoryPath(saveNumber);
        string path = Path.Combine(Application.persistentDataPath, $"{dirPath}/PlayerData{saveNumber:D2}.json");

        return path;
    }

    private void LoadAllSaveData()
    {
        _playerDatas.Clear();

        for (int i = 0; i < dataManager.MaxSaveSlot; i++)
        {
            PlayerData data = LoadData(i);

            if (data != null)
            {
                _playerDatas.TryAdd(i, data);
            }
        }
    }

    private PlayerData LoadData(int saveNumber)
    {
        try
        {
            string path = GetSaveDataPath(saveNumber);
            PlayerData data = null;

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<PlayerData>(json, dataManager.Settings);
            }

            return data;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }
}
