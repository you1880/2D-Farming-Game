using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using Data.Player;
using Data.Tile;
using UnityEngine;

public class SeedAction : IToolAction
{
    public int ToolRange => 1;
    private const string SUFFIX = " Seed";
    private Item _item;
    private InventoryItem _inventoryItem;
    private Define.CropType _cropType;

    public SeedAction(Item item, InventoryItem inventoryItem)
    {
        _item = item;
        _inventoryItem = inventoryItem;
        _cropType = GetCropNameFromItem();
    }

    public bool CanExecuteTool(Vector3 mousePosition, Vector3 userPosition)
    {
        if (!Util.IsToolReachable(mousePosition, userPosition, ToolRange))
        {
            return false;
        }

        Vector3Int tilePosition;
        FarmTile farmTile;

        if (!TryGetValidFarmTile(mousePosition, out farmTile, out tilePosition))
        {
            return false;
        }

        Data.Game.Crop crop = Managers.Data.GameDataManager.GetCropData(_cropType);
        if (crop == null)
        {
            return false;
        }

        return farmTile.isPlowed;
    }

    public void ExecuteAction(Vector3 mousePosition)
    {
        if (_inventoryItem == null)
        {
            return;
        }

        Vector3Int tilePosition;
        FarmTile farmTile;

        if (!TryGetValidFarmTile(mousePosition, out farmTile, out tilePosition))
        {
            return;
        }

        if (Managers.Data.InventoryDataManager.UseQuickSlotItem())
        {
            FarmTile newFarmTile = new FarmTile(farmTile.x, farmTile.y, farmTile.isPlowed, farmTile.isWatered, new PlantedCrop(_cropType, 0, 0, false));
            Managers.Data.TileDataManager.SetChangedTile(tilePosition, newFarmTile);
        }
    }

    private Define.CropType GetCropNameFromItem()
    {
        if (_item == null)
        {
            return Define.CropType.None;
        }

        string cropName = _item.itemName;

        if (cropName.EndsWith(SUFFIX))
        {
            cropName = cropName.Substring(0, cropName.Length - SUFFIX.Length);
        }

        if (Enum.TryParse(cropName, out Define.CropType cropType))
        {
            return cropType;
        }

        return Define.CropType.None;
    }
    
    private bool TryGetValidFarmTile(Vector3 mousePosition, out FarmTile farmTile, out Vector3Int tilePos)
    {
        tilePos = Managers.Tile.ConvertWorldToCell(mousePosition);
        farmTile = null;

        if (!Managers.Data.TileDataManager.TryGetChangedTile(tilePos, out GridTile tile))
            return false;

        if (tile is not FarmTile farm || farm.plantedCrop != null)
            return false;

        farmTile = farm;

        return true;
    }
}
