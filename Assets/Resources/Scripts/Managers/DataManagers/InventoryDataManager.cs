using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using Data.Player;
using UnityEngine;

public class InventoryDataManager
{
    private readonly IItemContainer _playerContainer;
    private readonly IItemMeta _meta;
    private const int DEFAULT_INVENTORY_CAPACITY = 30;
    public event Action<int> OnInventoryChanged;
    public int CurrentQuickSlotId { get; set; } = 0;
    public IItemContainer PlayerContainer => _playerContainer;

    public InventoryDataManager(IItemContainer playerContainer = null, IItemMeta meta = null)
    {
        if (playerContainer == null)
        {
            playerContainer = new InventoryContainer(DEFAULT_INVENTORY_CAPACITY);
        }

        if (meta == null)
        {
            meta = new ItemMeta();
        }

        _playerContainer = playerContainer;
        _meta = meta;
        _playerContainer.OnSlotChanged += (slot) => OnInventoryChanged?.Invoke(slot);
    }

    public void InitInventory(PlayerData playerData)
    {
        if (playerData == null)
        {
            return;
        }

        InventorySaveMapper.FromSave(_playerContainer, playerData.playerInventory);
    }

    public void WriteBackToPlayerData(PlayerData playerData)
    {
        if (playerData == null)
        {
            return;
        }

        if (_playerContainer is InventoryContainer inventoryContainer)
        {
            playerData.playerInventory = InventorySaveMapper.ToSave(inventoryContainer);
        }
    }

    public InventoryItem GetInventoryItem(int slotId)
        => _playerContainer.TryGet(slotId, out InventoryItem inventoryItem) ? inventoryItem : null;

    public InventoryItem GetQuickSlotItem()
        => GetInventoryItem(CurrentQuickSlotId);


    public bool TryUseItemInventory(int itemCode, int quantity)
    {
        if (quantity <= 0 || _playerContainer == null)
        {
            return false;
        }

        int hasCount = 0;
        List<int> hasSlots = new List<int>();

        for (int slotId = 0; slotId < _playerContainer.Capacity; slotId++)
        {
            if (_playerContainer.TryGet(slotId, out InventoryItem inventoryItem) && inventoryItem != null && inventoryItem.itemCode == itemCode && inventoryItem.quantity > 0)
            {
                hasCount += inventoryItem.quantity;
                hasSlots.Add(slotId);

                if (hasCount >= quantity)
                {
                    break;
                }
            }
        }

        if (hasCount < quantity)
        {
            return false;
        }

        int req = quantity;
        foreach (int slotId in hasSlots)
        {
            if (!_playerContainer.TryGet(slotId, out InventoryItem inventoryItem) || inventoryItem == null || inventoryItem.itemCode != itemCode || inventoryItem.quantity <= 0)
            {
                continue;
            }

            int sub = Math.Min(inventoryItem.quantity, req);
            int remain = inventoryItem.quantity - sub;

            if (remain <= 0)
            {
                _playerContainer.Remove(slotId);
            }
            else
            {
                inventoryItem.quantity = remain;
                _playerContainer.Set(slotId, inventoryItem);
            }

            req -= sub;
            if (req == 0)
            {
                break;
            }
        }

        return true;
    }

    public bool UseQuickSlotItem(int quantity = 1)
    {
        if (!_playerContainer.TryGet(CurrentQuickSlotId, out InventoryItem inventoryItem))
        {
            return false;
        }

        if (!TryUseItemInventory(inventoryItem.itemCode, quantity))
        {
            return false;
        }

        return true;
    }

    public bool HasAtLeastItemInInventory(int itemCode, int quantity)
        => ContainerUtil.HasAtLeastItem(_playerContainer, itemCode, quantity);

    public bool HasAtLeastItemsInInventory(IReadOnlyList<(int itemCode, int quantity)> requires)
        => ContainerUtil.HasAtLeastItems(_playerContainer, requires);

    public void SwapInventorySlot(int selectedSlotId, int targetSlotId)
        => SwapSlot(_playerContainer, selectedSlotId, _playerContainer, targetSlotId);

    public void SwapChestSlot(ChestContainer selectItemContainer, int selectedSlotId, ChestContainer targetItemContainer, int targetSlotId)
        => SwapSlot(selectItemContainer, selectedSlotId, targetItemContainer, targetSlotId);

    public void SwapBetweenCTI(IItemContainer itemContainer, int selectedSlotId, int targetSlotId)
        => SwapSlot(itemContainer, selectedSlotId, _playerContainer, targetSlotId);

    public void SwapBetweenITC(IItemContainer itemContainer, int selectedSlotId, int targetSlotId)
        => SwapSlot(_playerContainer, selectedSlotId, itemContainer, targetSlotId);

    private void SwapSlot(IItemContainer selectItemContainer, int selectedSlotId, IItemContainer targetItemContainer, int targetSlotId)
    {
        if (!IsValidSlot(selectItemContainer, selectedSlotId, targetItemContainer, targetSlotId))
        {
            return;
        }

        ContainerUtil.SwapOrMerge(selectItemContainer, selectedSlotId, targetItemContainer, targetSlotId, _meta);
    }

    private bool IsValidSlot(IItemContainer selectItemContainer, int selectedSlotId, IItemContainer targetItemContainer, int targetSlotId)
    {
        return selectItemContainer != null && selectedSlotId >= 0 && selectedSlotId < selectItemContainer.Capacity
            && targetItemContainer != null && targetSlotId >= 0 && targetSlotId < targetItemContainer.Capacity
            && (selectedSlotId != targetSlotId);
    }

    public bool AddItemInventory(int itemCode, int quantity, Define.ItemGrade itemGrade = Define.ItemGrade.None)
    {
        if (itemCode == 0 || quantity <= 0)
        {
            return false;
        }

        int maxStackSize = _meta.GetStackSize(itemCode);
        if (maxStackSize <= 0)
        {
            maxStackSize = 1;
        }

        int left = quantity;
        List<(int, int)> fillSlot = new List<(int slot, int put)>();
        List<(int, int)> newSlot = new List<(int, int)>();

        for (int slotId = 0; slotId < _playerContainer.Capacity && left > 0; slotId++)
        {
            if (!_playerContainer.TryGet(slotId, out InventoryItem inventoryItem))
            {
                continue;
            }

            if (!_meta.CanStack(inventoryItem, new InventoryItem(itemCode, 1, itemGrade)))
            {
                continue;
            }

            int stack = maxStackSize - inventoryItem.quantity;
            if (stack <= 0)
            {
                continue;
            }

            int put = Math.Min(stack, left);
            fillSlot.Add((slotId, put));
            left -= put;
        }

        for (int slotId = 0; slotId < _playerContainer.Capacity && left > 0; slotId++)
        {
            if (_playerContainer.Contains(slotId))
            {
                continue;
            }

            int put = Math.Min(maxStackSize, left);
            newSlot.Add((slotId, put));
            left -= put;
        }

        if (left > 0)
        {
            return false;
        }

        foreach (var (slot, put) in fillSlot)
        {
            _playerContainer.TryGet(slot, out InventoryItem inventoryItem);

            if (inventoryItem == null)
            {
                continue;
            }

            inventoryItem.quantity += put;
            _playerContainer.Set(slot, inventoryItem);
        }

        foreach (var (slot, put) in newSlot)
        {
            InventoryItem inventoryItem = new InventoryItem(itemCode, put, itemGrade);
            _playerContainer.Set(slot, inventoryItem);
        }

        return true;
    }

    public void RemoveInventoryItem(int slotId)
    {
        if (_playerContainer.TryGet(slotId, out InventoryItem inventoryItem))
        {
            _playerContainer.Remove(slotId);
        }
    }

    public bool TryRemoveAt(int slotId, int quantity)
    {
        if (!_playerContainer.TryGet(slotId, out InventoryItem inventoryItem) || inventoryItem == null || quantity <= 0)
        {
            return false;
        }

        int remain = inventoryItem.quantity - quantity;
        if (remain <= 0)
        {
            _playerContainer.Remove(slotId);
        }
        else
        {
            inventoryItem.quantity = remain;
            _playerContainer.Set(slotId, inventoryItem);
        }

        return true;
    }

    public bool TrySplitItem(InventoryItem fromInventoryItem, int fromSlotId, int splitQuantity, IItemContainer itemContainer = null)
    {
        if (fromInventoryItem == null || splitQuantity <= 0)
        {
            return false;
        }
        
        itemContainer = itemContainer == null ? _playerContainer : itemContainer;

        for (int slotId = 0; slotId < itemContainer.Capacity; slotId++)
        {
            if (itemContainer.Contains(slotId)) continue;

            InventoryItem remainItem = new InventoryItem(fromInventoryItem.itemCode, fromInventoryItem.quantity - splitQuantity, fromInventoryItem.itemGrade);
            InventoryItem splitItem = new InventoryItem(fromInventoryItem.itemCode, splitQuantity, fromInventoryItem.itemGrade);

            itemContainer.Set(fromSlotId, remainItem);
            itemContainer.Set(slotId, splitItem);

            return true;
        }

        return false;
    }
}
