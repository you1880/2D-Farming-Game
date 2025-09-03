using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using Data.Tile;
using UnityEngine;

public class FurnitureAction : IToolAction
{
    public int ToolRange => 1;
    private readonly Dictionary<int, Define.FurnitureType> _furnitureMaps = new Dictionary<int, Define.FurnitureType>
    {
        {10001, Define.FurnitureType.Chest},
        {10002, Define.FurnitureType.Furnace}
    };
    private InventoryItem _inventoryItem;
    private Define.FurnitureType _furnitureType;
    private Vector3Int _tilePos;

    public FurnitureAction(Item item, InventoryItem inventoryItem)
    {
        _inventoryItem = inventoryItem;
        _furnitureType = _furnitureMaps.TryGetValue(item.itemCode, out Define.FurnitureType ft)
            ? ft : Define.FurnitureType.None;
    }
    
    public bool CanExecuteTool(Vector3 mousePosition, Vector3 userPosition)
    {
        if (!Util.IsToolReachable(mousePosition, userPosition, ToolRange))
        {
            return false;
        }

        Collider2D hitObject = Physics2D.OverlapPoint(mousePosition, ~(1 << 2));
        if (hitObject != null)
        {
            return false;
        }

        Vector3Int tilePos = Managers.Tile.ConvertWorldToCell(mousePosition);

        if (!Managers.Tile.GrassTilemap.HasTile(tilePos))
        {
            return false;
        }

        if (Managers.Data.TileDataManager.TryGetChangedTile(tilePos, out GridTile tile) && tile is PropTile propTile && propTile.prop != null)
        {
            return false;
        }

        _tilePos = tilePos;

        return true;
    }

    public void ExecuteAction(Vector3 mousePosition)
    {
        if (_inventoryItem == null || _furnitureType == Define.FurnitureType.None)
        {
            return;
        }
        Data.Prop.Furniture furniture = null;

        switch (_inventoryItem.itemCode)
        {
            case 10001:
                furniture = new Data.Prop.Chest(Managers.Data.ContainerService.CreateChestContainer());
                break;
            case 10002:
                furniture = new Data.Prop.Furnace();
                break;
        }

        if (Managers.Data.InventoryDataManager.UseQuickSlotItem())
        {
            PropTile propTile = new PropTile(_tilePos.x, _tilePos.y, furniture);
            Managers.Data.TileDataManager.SetChangedTile(_tilePos, propTile);
        }
    }
}
