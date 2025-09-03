using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using Data.Player;
using UnityEngine;

public static class ContainerUtil
{
    public static void SwapOrMerge(IItemContainer containerA, int slotA, IItemContainer containerB, int slotB, IItemMeta meta)
    {
        containerA.TryGet(slotA, out InventoryItem inventoryItemA);
        containerB.TryGet(slotB, out InventoryItem inventoryItemB);
        Debug.Log($"Swap : {inventoryItemA == null} / {inventoryItemB == null}");
        if (inventoryItemA == null && inventoryItemB == null)
        {
            return;
        }

        if (inventoryItemA != null && inventoryItemB == null)
        {
            containerA.Remove(slotA);
            containerB.Set(slotB, inventoryItemA);

            return;
        }

        if (inventoryItemA == null && inventoryItemB != null)
        {
            containerB.Remove(slotB);
            containerA.Set(slotA, inventoryItemB);

            return;
        }

        if (meta.CanStack(inventoryItemA, inventoryItemB))
        {
            int maxStackSize = meta.GetStackSize(inventoryItemA.itemCode);
            int totalQuantity = inventoryItemA.quantity + inventoryItemB.quantity;

            if (totalQuantity <= maxStackSize)
            {
                inventoryItemB.quantity = totalQuantity;
                containerB.Set(slotB, inventoryItemB);
                containerA.Remove(slotA);
            }
            else
            {
                inventoryItemB.quantity = maxStackSize;
                containerB.Set(slotB, inventoryItemB);

                inventoryItemA.quantity = totalQuantity - maxStackSize;
                containerA.Set(slotA, inventoryItemA);
            }

            return;
        }

        containerA.Set(slotA, inventoryItemB);
        containerB.Set(slotB, inventoryItemA);
    }

    public static int CountContainerItem(this IItemContainer container, int itemCode)
    {
        if (container == null)
        {
            return 0;
        }

        int count = 0;
        for (int slotId = 0; slotId < container.Capacity; slotId++)
        {
            if (container.TryGet(slotId, out InventoryItem inventoryItem) && inventoryItem != null && inventoryItem.itemCode == itemCode)
            {
                count += inventoryItem.quantity;
            }
        }

        return count;
    }

    public static Dictionary<int, int> CountAllContainerItems(this IItemContainer container)
    {
        Dictionary<int, int> counts = new Dictionary<int, int>();
        if (container == null)
        {
            return counts;
        }

        for (int slotId = 0; slotId < container.Capacity; slotId++)
        {
            if (!container.TryGet(slotId, out InventoryItem inventoryItem) || inventoryItem == null)
            {
                continue;
            }

            if (counts.TryGetValue(inventoryItem.itemCode, out int q))
                counts[inventoryItem.itemCode] = q + inventoryItem.quantity;
            else
                counts[inventoryItem.itemCode] = inventoryItem.quantity;

        }

        return counts;
    }

    public static bool HasAtLeastItem(IItemContainer container, int itemCode, int count)
        => container.CountContainerItem(itemCode) >= count;

    public static bool HasAtLeastItems(IItemContainer container, List<(int itemCode, int quantity)> requires)
    {
        Dictionary<int, int> dict = CountAllContainerItems(container);

        foreach (var req in requires)
        {
            if (!dict.ContainsKey(req.itemCode) || dict[req.itemCode] < req.quantity)
            {
                return false;
            }
        }

        return true;
    }
}