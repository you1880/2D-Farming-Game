using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using UnityEngine;

public interface IItemMeta
{
    int GetStackSize(int itemCode);
    bool CanStack(InventoryItem a, InventoryItem b);
}
