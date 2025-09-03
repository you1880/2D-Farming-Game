using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Data.Player;
using Newtonsoft.Json;
using System;
using Data.Tile;

public class TileDataManager
{
    private DataManager dataManager => Managers.Data;
    private PlayerTileData tileData;
    private Dictionary<Vector2Int, GridTile> _changedTiles = new Dictionary<Vector2Int, GridTile>();
    public IReadOnlyDictionary<Vector2Int, GridTile> ChangedTiles => _changedTiles;
    public Action<Vector2Int> OnTileDataChanged;

    public bool TryGetChangedTile(Vector3Int tilePosition, out GridTile tile)
        => _changedTiles.TryGetValue(new Vector2Int(tilePosition.x, tilePosition.y), out tile);

    public bool TryGetChangedTile(Vector2Int tilePosition, out GridTile tile)
        => _changedTiles.TryGetValue(tilePosition, out tile);

    public void SetChangedTile(Vector3Int tilePosition, GridTile tile)
    {
        Vector2Int pos = new Vector2Int(tilePosition.x, tilePosition.y);

        _changedTiles[pos] = tile;
        OnTileDataChanged?.Invoke(pos);
    }

    public void SetChangedTile(Vector2Int tilePosition, GridTile tile)
    {
        _changedTiles[tilePosition] = tile;
        OnTileDataChanged?.Invoke(tilePosition);
    }

    public void SetCurrentTileData(PlayerTileData playerTileData)
    {
        if (playerTileData == null || tileData == playerTileData)
        {
            return;
        }

        tileData = playerTileData;
        InitTileDict();
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
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void SaveTileDataToJson(int saveNumber, PlayerTileData data = null)
    {
        if (!dataManager.IsValidSlotIndex(saveNumber))
        {
            return;
        }

        try
        {
            PlayerTileData saveData = data ?? tileData;
            if (saveData == null)
            {
                Debug.Log("NULL");
                return;
            }

            saveData.tiles = GetTileChanges();

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, dataManager.Settings);
            string path = GetSaveDataPath(saveNumber);

            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public PlayerTileData LoadTileData(int saveNumber)
    {
        try
        {
            string path = GetSaveDataPath(saveNumber);
            PlayerTileData data = null;

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<PlayerTileData>(json, dataManager.Settings);
            }

            return data;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    // CropTile => IsPlowed / Crop 없으면 삭제
    // PropTile -> Prop이 null이면 삭제
    private List<GridTile> GetTileChanges()
    {
        List<GridTile> tiles = new List<GridTile>();

        foreach (GridTile tile in _changedTiles.Values)
        {
            if (tile is PropTile propTile && propTile.prop != null)
            {
                if (propTile.prop is Data.Prop.Chest chest)
                {
                    Data.Prop.Chest c = Managers.Data.ContainerService.SaveChestContainer(chest.chestGuid);
                    propTile.prop = c;
                }

                tiles.Add(tile);
            }
            else if (tile is FarmTile farmTile)
            {
                if (farmTile.isPlowed || (farmTile.plantedCrop != null && farmTile.plantedCrop.cropType != Define.CropType.None))
                {
                    tiles.Add(tile);
                }
            }
        }

        return tiles;
    }

    private string GetSaveDataPath(int saveNumber)
    {
        string dirPath = dataManager.GetDirectoryPath(saveNumber);
        string path = Path.Combine(Application.persistentDataPath, $"{dirPath}/TileData_{saveNumber:D2}.json");

        return path;
    }

    private void InitTileDict()
    {
        _changedTiles.Clear();

        if (tileData == null)
        {
            return;
        }

        foreach (GridTile tile in tileData.tiles)
        {
            Vector2Int tilePos = new Vector2Int(tile.x, tile.y);
            if (CheckTile(tile))
            {
                _changedTiles[tilePos] = tile;
            }
        }
    }

    private bool CheckTile(GridTile tile)
    {
        if (tile is PropTile propTile)
        {
            return true;
        }
        
        if (tile is not FarmTile farmTile)
        {
            return false;
        }

        return farmTile.isPlowed || (farmTile.plantedCrop != null && farmTile.plantedCrop.cropType != Define.CropType.None);
    }
}
