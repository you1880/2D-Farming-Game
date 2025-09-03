using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using UnityEngine;

public static class InventorySaveMapper
{
    public static PlayerInventorySave ToSave(IItemContainer container)
    {
        PlayerInventorySave save = new PlayerInventorySave
        {
            inventorySize = container.Capacity,
            inventory = new List<InventoryItemSave>()
        };

        for (int slotId = 0; slotId < container.Capacity; slotId++)
        {
            if (container.TryGet(slotId, out InventoryItem inventoryItem))
            {
                save.inventory.Add(new InventoryItemSave(slotId, inventoryItem.itemCode, inventoryItem.quantity, inventoryItem.itemGrade));
            }
        }

        return save;
    }

    public static void FromSave(IItemContainer container, PlayerInventorySave save)
    {
        if (save == null || save.inventory == null)
        {
            return;
        }

        container.RemoveAll();

        foreach (InventoryItemSave item in save.inventory)
        {
            if (item.slotId < 0 || item.slotId >= container.Capacity)
            {
                continue;
            }

            InventoryItem inventoryItem = new InventoryItem(item.itemCode, item.quantity, item.itemGrade);
            container.Set(item.slotId, inventoryItem);
        }
    }

}
