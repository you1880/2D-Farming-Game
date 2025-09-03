using System.Collections;
using System.Collections.Generic;
using Data.Tile;
using UnityEngine;

public class ShovelAction : IToolAction
{
    public int ToolRange => 1;

    public bool CanExecuteTool(Vector3 mousePosition, Vector3 userPosition)
    {
        return Util.IsToolReachable(mousePosition, userPosition, ToolRange);
    }

    public void ExecuteAction(Vector3 mousePosition)
    {
        Vector3Int tilePosition = Managers.Tile.ConvertWorldToCell(mousePosition);

        if (!Managers.Data.TileDataManager.TryGetChangedTile(tilePosition, out GridTile tile))
        {
            return;
        }

        if (tile is not FarmTile farmTile)
        {
            return;
        }

        FarmTile newFarmTile;

        if (farmTile.plantedCrop != null)
        {
            newFarmTile = new FarmTile(farmTile.x, farmTile.y, farmTile.isPlowed, farmTile.isWatered, null);
        }
        else
        {
            newFarmTile = new FarmTile(farmTile.x, farmTile.y, false, false, farmTile.plantedCrop);
        }

        Managers.Data.TileDataManager.SetChangedTile(tilePosition, newFarmTile);
    }
}
