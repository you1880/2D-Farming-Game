using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using UnityEngine;

public class ItemMeta : IItemMeta
{
    public int GetStackSize(int itemCode)
    {
        Item item = Managers.Data.GameDataManager.GetItemData(itemCode);

        return item == null ? 0 : item.maxStackSize;
    }

    public bool CanStack(InventoryItem inventoryItemA, InventoryItem inventoryItemB)
    {
        if (inventoryItemA == null || inventoryItemB == null)
        {
            return false;
        }

        int maxStackSize = GetStackSize(inventoryItemA.itemCode);

        return inventoryItemA.itemCode == inventoryItemB.itemCode
            && inventoryItemA.itemGrade == inventoryItemB.itemGrade;
    }

}
