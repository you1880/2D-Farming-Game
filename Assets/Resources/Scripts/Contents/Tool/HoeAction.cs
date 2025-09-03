using System.Collections;
using System.Collections.Generic;
using Data.Tile;
using UnityEngine;

public class HoeAction : IToolAction
{
    public int ToolRange => 1;

    public bool CanExecuteTool(Vector3 mousePosition, Vector3 userPosition)
    {
        return Util.IsToolReachable(mousePosition, userPosition, ToolRange);
    }

    public void ExecuteAction(Vector3 mousePosition)
    {
        Vector3Int tilePosition = Managers.Tile.ConvertWorldToCell(mousePosition);

        if(Managers.Data.TileDataManager.TryGetChangedTile(tilePosition, out GridTile tile))
        {
            return;
        }

        FarmTile farmTile = new FarmTile(tilePosition.x, tilePosition.y, true, false, null);
        Managers.Data.TileDataManager.SetChangedTile(tilePosition, farmTile);
    }
}
