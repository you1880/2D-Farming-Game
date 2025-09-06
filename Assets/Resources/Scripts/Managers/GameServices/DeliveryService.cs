using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using UnityEngine;

public class DeliveryService
{
    private Dictionary<(int itemCode, Define.ItemGrade itemGrade), int> _deliveryItems = new Dictionary<(int itemCode, Define.ItemGrade itemGrade), int>();
    private InventoryItem _lastInputItem = null;
    private InventoryDataManager inventoryDataManager => Managers.Data.InventoryDataManager;

    public bool TryAddDeliveryItem()
    {
        InventoryItem quickSlotItem = inventoryDataManager.GetQuickSlotItem();
        int quickSlotId = inventoryDataManager.CurrentQuickSlotId;

        if (quickSlotItem == null)
        {
            return false;
        }

        Item item = Managers.Data.GameDataManager.GetItemData(quickSlotItem.itemCode);
        if (item == null || item.sellingCost == 0)
        {
            return false;
        }

        if (!inventoryDataManager.TryRemoveAt(quickSlotId, quickSlotItem.quantity))
        {
            return false;
        }

        int itemCode = quickSlotItem.itemCode;
        int quantity = quickSlotItem.quantity;
        Define.ItemGrade itemGrade = quickSlotItem.itemGrade;

        _deliveryItems[(itemCode, itemGrade)] = (_deliveryItems.TryGetValue((itemCode, itemGrade), out int q) ? q : 0) + quantity;
        _lastInputItem = new InventoryItem(itemCode, quantity, itemGrade);

        return true;
    }

    public bool GetAndAddLastInputItem()
    {
        if (_lastInputItem == null)
        {
            return false;
        }

        if (!inventoryDataManager.AddItemInventory(_lastInputItem.itemCode, _lastInputItem.quantity, _lastInputItem.itemGrade))
        {
            return false;
        }

        _lastInputItem = null;

        return true;
    }

    public List<InventoryItem> GetInputItemList()
    {
        List<InventoryItem> inputItems = new List<InventoryItem>();

        foreach (var item in _deliveryItems)
        {
            int itemCode = item.Key.itemCode;
            int quantity = item.Value;
            Define.ItemGrade itemGrade = item.Key.itemGrade;

            inputItems.Add(new InventoryItem(itemCode, quantity, itemGrade));
        }

        return inputItems;
    }

    public void ClearInputItemList()
    {
        _deliveryItems.Clear();
    }
}
