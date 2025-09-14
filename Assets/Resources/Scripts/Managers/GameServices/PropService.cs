using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Tile;
using UnityEngine;

public class PropService
{
    public bool TryBreakProp(Vector3Int tilePosition, Define.PropType propType, Data.Game.PropDropTable propDropTable = null)
    {
        PropDropTable dropTable = propDropTable ?? Managers.Data.GameDataManager.GetPropDropTable(propType);

        if (dropTable == null)
        {
            return false;
        }

        if (!Managers.Data.TileDataManager.TryGetChangedTile(tilePosition, out GridTile tile))
        {
            return false;
        }

        if (tile is not PropTile propTile || propTile.prop == null)
        {
            return false;
        }

        foreach (DropItem dropItem in dropTable.dropItems)
        {
            SpawnDropItem(tilePosition, dropItem);
        }

        Managers.Data.TileDataManager.SetChangedTile(tilePosition, null);
        return true;
    }

    public bool TryBreakProp(Vector3 propPosition, Define.PropType propType, Data.Game.PropDropTable propDropTable = null)
    {
        PropDropTable dropTable = propDropTable ?? Managers.Data.GameDataManager.GetPropDropTable(propType);

        if (dropTable == null)
        {
            return false;
        }

        foreach (DropItem dropItem in dropTable.dropItems)
        {
            if (UnityEngine.Random.value < dropItem.dropChance)
            {
                SpawnDropItem(Managers.Tile.ConvertWorldToCell(propPosition), dropItem);
            }
        }

        return true;
    }

    public void SpawnDropItem(Vector3Int tilePosition, DropItem dropItem)
    {
        if (dropItem == null || dropItem.itemCode == 0)
        {
            return;
        }

        int quantity = UnityEngine.Random.Range(dropItem.minDropCount, dropItem.maxDropCount + 1);
        Item item = Managers.Data.GameDataManager.GetItemData(dropItem.itemCode);
        int stackSize = Math.Max(1, item?.maxStackSize ?? 1);

        void SpawnStacks(int total, Define.ItemGrade grade)
        {
            while (total > 0)
            {
                int spawnQuantity = Math.Min(total, stackSize);
                Managers.Prop.SpawnProp(new PropSpawnData
                {
                    tilePosition = tilePosition,
                    propType = Define.PropType.DropItem,
                    itemCode = dropItem.itemCode,
                    itemQuantity = spawnQuantity,
                    itemGrade = grade
                });

                total -= spawnQuantity;
            }
        }

        if (dropItem.useGrade)
        {
            Dictionary<Define.ItemGrade, int> pairs = new Dictionary<Define.ItemGrade, int>();
            for (int i = 0; i < quantity; i++)
            {
                Define.ItemGrade grade = Util.ItemGradeRNG(dropItem.dropItemGradeWeights);
                pairs[grade] = (pairs.TryGetValue(grade, out int q) ? q : 0) + 1;
            }

            foreach (var kv in pairs)
            {
                SpawnStacks(kv.Value, kv.Key);
            }
        }
        else
        {
            SpawnStacks(quantity, Define.ItemGrade.None);
        }
    }
}
