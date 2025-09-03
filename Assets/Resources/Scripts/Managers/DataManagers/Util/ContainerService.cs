using System;
using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using UnityEngine;

public class ContainerService
{
    private readonly Dictionary<string, ChestContainer> _chest = new Dictionary<string, ChestContainer>();

    public string CreateChestContainer(string chestId = null, int capacity = 30)
    {
        string id = string.IsNullOrEmpty(chestId) ? Guid.NewGuid().ToString("N") : chestId;

        if (!_chest.ContainsKey(id))
        {
            _chest[id] = new ChestContainer(capacity);
        }
        Debug.Log($"Chest Id : {id} / {_chest.Count}");
        return id;
    }

    public ChestContainer GetChestContainer(string chestId)
        => _chest.TryGetValue(chestId, out ChestContainer chestContainer) ? chestContainer : null;

    public bool DeleteChestContainer(string chestId)
        => _chest.Remove(chestId);

    // Data.Chest를 반환하여 PropTile의 Furniture.Furniture에 저장
    public Data.Prop.Chest SaveChestContainer(string chestId)
    {
        ChestContainer chestContainer = GetChestContainer(chestId);

        if (chestContainer == null)
        {
            return null;
        }

        Data.Prop.Chest chest = new Data.Prop.Chest(chestId);

        for (int slotId = 0; slotId < chestContainer.Capacity; slotId++)
        {
            if (chestContainer.TryGet(slotId, out InventoryItem inventoryItem))
            {
                chest.chestItems.Add(new InventoryItemSave(slotId, inventoryItem.itemCode, inventoryItem.quantity, inventoryItem.itemGrade));
            }
        }

        return chest;
    }

    public void LoadChestContainer(Data.Prop.Chest chest)
    {
        if (chest == null)
        {
            return;
        }

        ChestContainer chestContainer = new ChestContainer();

        foreach (InventoryItemSave item in chest.chestItems)
        {
            InventoryItem inventoryItem = new InventoryItem(item.itemCode, item.quantity, item.itemGrade);
            chestContainer.Set(item.slotId, inventoryItem);
        }

        _chest[chest.chestGuid] = chestContainer;
    }
}
