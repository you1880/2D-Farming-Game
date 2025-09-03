using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Tile;
using UnityEngine;

public class FarmingService 
{
    private readonly PropService _propService;
    public FarmingService(PropService propService)
    {
        _propService = propService;
    }

    public void TryHarvestCropAtPosition(Vector3 mousePosition, Vector3 userPosition)
    {
        if (!Util.IsToolReachable(mousePosition, userPosition))
        {
            return;
        }

        Vector3Int tilePosition = Managers.Tile.ConvertWorldToCell(mousePosition);
        if (!Managers.Data.TileDataManager.TryGetChangedTile(tilePosition, out GridTile tile))
        {
            return;
        }

        if (tile is not FarmTile farmTile || farmTile.plantedCrop == null)
        {
            return;
        }

        Data.Game.Crop crop = Managers.Data.GameDataManager.GetCropData(farmTile.plantedCrop.cropType);

        if (farmTile.plantedCrop.growLevel >= crop?.requireDays)
        {
            HarvestCrop(tilePosition, farmTile, crop);
        }
    }

    private void HarvestCrop(Vector3Int tilePosition, FarmTile farmTile, Data.Game.Crop cropData)
    {
        PlantedCrop plantedCrop = farmTile.plantedCrop;

        if (cropData.isRepeatable)
        {
            plantedCrop.growLevel -= cropData.repeatableRequireDays;

            GameObject cropObject = Managers.Prop.GetPropObject(tilePosition);
            if (cropObject != null)
            {
                Crop crop = cropObject.GetComponent<Crop>();
                crop?.SetCropStatus(plantedCrop.cropType, plantedCrop.growLevel);
            }
        }
        else
        {
            plantedCrop = null;
            Managers.Prop.RemovePropFromDict(tilePosition);
        }

        Managers.Data.TileDataManager.SetChangedTile(tilePosition, new FarmTile(tilePosition.x, tilePosition.y, farmTile.isPlowed, farmTile.isWatered, plantedCrop));
        DropHarvestedItem(tilePosition, cropData);
    }

    private void DropHarvestedItem(Vector3Int tilePosition, Data.Game.Crop cropData)
    {
        CropDropTable cropDropTable = Managers.Data.GameDataManager.GetCropDropTable(cropData.cropType);
        if (cropDropTable == null)
        {
            return;
        }

        foreach (DropItem dropItem in cropDropTable.dropItems)
        {
            _propService.SpawnDropItem(tilePosition, dropItem);
        }
    }

    public FarmTile GrowCrop(FarmTile farmTile)
    {
        if (farmTile == null)
        {
            return null;
        }

        if (farmTile.plantedCrop == null || farmTile.plantedCrop.cropType == Define.CropType.None)
        {
            return new FarmTile(farmTile.x, farmTile.y, farmTile.isPlowed, false, null);
        }

        Data.Game.Crop crop = Managers.Data.GameDataManager.GetCropData(farmTile.plantedCrop.cropType);
        PlantedCrop plantedCrop = farmTile.plantedCrop;

        if (farmTile.isWatered)
        {
            plantedCrop.growLevel = Mathf.Min(plantedCrop.growLevel + 1, crop.requireDays);
            plantedCrop.noWateredDays = 0;
        }
        else if (!farmTile.isWatered && plantedCrop.growLevel < crop.requireDays)
        {
            plantedCrop.noWateredDays += 1;

            if (plantedCrop.noWateredDays >= 3)
            {
                plantedCrop.isDead = true;
            }
        }

        return new FarmTile(farmTile.x, farmTile.y, farmTile.isPlowed, false, plantedCrop);
    }
}
