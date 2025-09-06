using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Player;
using Data.Tile;
using UnityEngine;

public class GameFlowService
{
    private UserDataManager userDataManager => Managers.Data.UserDataManager;
    private readonly FarmingService _farmingService;
    private readonly DeliveryService _deliveryService;
    public GameFlowService(FarmingService farmingService, DeliveryService deliveryService)
    {
        _farmingService = farmingService;
        _deliveryService = deliveryService;
    }

    public void StartGame(int saveNumber)
    {
        if (Managers.Data.LoadData(saveNumber))
        {
            Managers.Scene.LoadNextScene(Define.SceneType.Main);
        }
        else
        {
            Managers.UI.ShowMessageBoxUI(MessageID.LoadDataError, null, true);
        }
    }

    public void StartNewGame(int saveNumber, string name, string farmName, Define.PlayerGender gender)
    {
        PlayerData playerData = userDataManager.CreateNewData(name, farmName, gender);
        PlayerTileData playerTileData = new PlayerTileData();

        Managers.Data.SaveData(saveNumber, playerData, playerTileData);
        Managers.Data.SetCurrentData(saveNumber, playerData, playerTileData);

        Managers.Scene.LoadNextScene(Define.SceneType.Main);
    }

    public void DayEnd()
    {
        var tiles = Managers.Data.TileDataManager.ChangedTiles.ToArray();

        foreach (var tile in tiles)
        {
            if (tile.Value is FarmTile farmTile)
            {
                Managers.Data.TileDataManager.SetChangedTile(tile.Key, _farmingService.GrowCrop(farmTile));
            }
        }

        _deliveryService.ClearInputItemList();
        
        Managers.Area.SetCurrentArea(Define.Area.FarmHouse);
        Managers.Prop.ClearHarvestedTrees();
        Managers.Data.SaveData(Managers.Data.CurrentDataNumber);
        Managers.Scene.LoadNextScene(Define.SceneType.Main);
    }
}
