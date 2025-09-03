using System;
using System.Collections;
using System.IO;
using Data.Player;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager
{
    public GameDataManager GameDataManager { get; } = new GameDataManager();
    public UserDataManager UserDataManager { get; } = new UserDataManager();
    public TileDataManager TileDataManager { get; } = new TileDataManager();
    public InventoryDataManager InventoryDataManager = new InventoryDataManager();
    public ContainerService ContainerService = new ContainerService();
    public JsonSerializerSettings Settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
    public int CurrentDataNumber { get; set; } = -1;
    public int MaxSaveSlot = 5;

    public void Init()
    {
        GameDataManager.Init();
        UserDataManager.Init();
    }

    public bool LoadData(int saveNumber)
    {
        if (!IsValidSlotIndex(saveNumber))
        {
            return false;
        }

        PlayerData playerData = UserDataManager.GetSaveData(saveNumber);
        PlayerTileData playerTileData = TileDataManager.LoadTileData(saveNumber);

        if (playerData == null || playerTileData == null)
        {
            Debug.Log($"{playerData == null} / {playerTileData == null}");
            return false;
        }

        SetCurrentData(saveNumber, playerData, playerTileData);

        return true;
    }

    public void SetCurrentData(int saveNumber, PlayerData playerData = null, PlayerTileData playerTileData = null)
    {
        CurrentDataNumber = saveNumber;
        UserDataManager.SetCurrentSaveData(playerData);
        InventoryDataManager.InitInventory(playerData);
        TileDataManager.SetCurrentTileData(playerTileData);
    }

    public void SaveData(int saveNumber, PlayerData data = null, PlayerTileData playerTileData = null)
    {
        string dirPath = GetDirectoryPath(saveNumber);
        Directory.CreateDirectory(dirPath);

        UserDataManager.SaveDataToJson(saveNumber, data);
        TileDataManager.SaveTileDataToJson(saveNumber, playerTileData);
    }

    public void DeleteData(int saveNumber)
    {
        string dirPath = GetDirectoryPath(saveNumber);

        if (Directory.Exists(dirPath))
        {
            Directory.Delete(dirPath, true);
        }
    }

    public bool IsValidSlotIndex(int saveNumber)
    {
        return saveNumber >= 0 && saveNumber < MaxSaveSlot;
    }

    public string GetDirectoryPath(int saveNumber)
    {
        return Path.Combine(Application.persistentDataPath, $"Slot_{saveNumber:D2}");
    }
}
