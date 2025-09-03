using System;
using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using UnityEngine;

public interface IItemContainer
{
    bool TryGet(int slotId, out InventoryItem inventoryItem);
    void Set(int slotId, InventoryItem inventoryItem);
    void Remove(int slotId);
    void RemoveAll();
    bool Contains(int slotId);
    int Capacity { get; }
    event Action<int> OnSlotChanged;
}
