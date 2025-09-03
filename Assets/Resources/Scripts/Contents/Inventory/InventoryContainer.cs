using System;
using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using UnityEngine;

public class InventoryContainer : IItemContainer
{
    private readonly Dictionary<int, InventoryItem> _slots = new Dictionary<int, InventoryItem>();
    public int Capacity { get; }
    public event Action<int> OnSlotChanged;
    public InventoryContainer(int capacity) => Capacity = capacity;

    public bool TryGet(int slotId, out InventoryItem inventoryItem)
        => _slots.TryGetValue(slotId, out inventoryItem);

    public void Set(int slotId, InventoryItem inventoryItem)
    {
        _slots[slotId] = inventoryItem;
        OnSlotChanged?.Invoke(slotId);
    }

    public void Remove(int slotId)
    {
        if (_slots.Remove(slotId))
        {
            OnSlotChanged?.Invoke(slotId);
        }
    }

    public void RemoveAll()
    {
        if (_slots.Count == 0)
        {
            return;
        }

        _slots.Clear();
    }

    public bool Contains(int slotId)
        => _slots.ContainsKey(slotId);
}
