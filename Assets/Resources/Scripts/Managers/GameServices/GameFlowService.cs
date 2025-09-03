using System.Collections;
using System.Collections.Generic;
using Data.Player;
using Data.Tile;
using UnityEngine;

public class GameFlowService
{
    private UserDataManager userDataManager => Managers.Data.UserDataManager;
    private readonly FarmingService _farmingService;
    public GameFlowService(FarmingService farmingService)
    {
        _farmingService = farmingService;
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
        Dictionary<Vector2Int, GridTile> changedTiles = new Dictionary<Vector2Int, GridTile>();

        foreach (var tile in Managers.Data.TileDataManager.ChangedTiles)
        {
            if (tile.Value is FarmTile farmTile)
            {
                changedTiles[tile.Key] = _farmingService.GrowCrop(farmTile);
            }
        }

        foreach (var tile in changedTiles)
        {
            Managers.Data.TileDataManager.SetChangedTile(tile.Key, tile.Value);
        }

        Managers.Area.SetCurrentArea(Define.Area.FarmHouse);
        Managers.Data.SaveData(Managers.Data.CurrentDataNumber);
        Managers.Scene.LoadNextScene(Define.SceneType.Main);
    }
}
