using System.Collections;
using System.Collections.Generic;
using Data.Tile;
using UnityEngine;

public class WateringCanAction : IToolAction
{
    public int ToolRange => 1;

    public bool CanExecuteTool(Vector3 mousePosition, Vector3 userPosition)
    {
        return Util.IsToolReachable(mousePosition, userPosition, ToolRange);
    }

    public void ExecuteAction(Vector3 mousePosition)
    {
        Vector3Int tilePosition = Managers.Tile.ConvertWorldToCell(mousePosition);
        if(!Managers.Data.TileDataManager.TryGetChangedTile(tilePosition, out GridTile tile))
        {
            Debug.Log("No Tile");
            return;
        }

        if (tile is not FarmTile farmTile)
        {
            Debug.Log("Not Farm Tile");
            return;
        }

        FarmTile newFarmTile = new FarmTile(tilePosition.x, tilePosition.y, farmTile.isPlowed, true, farmTile.plantedCrop);
        Managers.Data.TileDataManager.SetChangedTile(tilePosition, newFarmTile);
    }
}
