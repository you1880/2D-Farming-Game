using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using Data.Player;
using UnityEngine;

public class UI_QuickSlot : UI_BaseInventory
{
    public enum GameObjects
    {
        QuickSlot0,
        QuickSlot1,
        QuickSlot2,
        QuickSlot3,
        QuickSlot4,
        QuickSlot5,
        QuickSlot6,
        QuickSlot7,
        QuickSlot8,
        SelectSlot
    }

    private const int QUICK_SLOT_START = 0;
    private const int QUICK_SLOT_END = 9;
    private GameObject _selectSlotObject;

    private void SetCurrentQuickSlot(int quickSlotId)
    {
        if (inventoryDataManager.CurrentQuickSlotId == quickSlotId)
        {
            return;
        }

        GameObject quickSlotObject = GetObject(quickSlotId);

        if (quickSlotObject == null)
        {
            return;
        }

        inventoryDataManager.CurrentQuickSlotId = quickSlotId;
        _selectSlotObject.transform.position = quickSlotObject.transform.position;
    }

    private void OnKeyboardEvent(Define.KeyBoardEvent keyEvent)
    {
        if (!(keyEvent >= Define.KeyBoardEvent.Num1 && keyEvent <= Define.KeyBoardEvent.Num9))
        {
            return;
        }

        int quickSlotId = (int)keyEvent - (int)Define.KeyBoardEvent.Num1;

        SetCurrentQuickSlot(quickSlotId);
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
    }

    private void GetUIElements()
    {
        _selectSlotObject = GetObject((int)GameObjects.SelectSlot);
    }

    private void InitSlot()
    {
        _inventorySlots.Clear();

        for (int slotId = (int)GameObjects.QuickSlot0; slotId <= (int)GameObjects.QuickSlot8; slotId++)
        {
            GameObject slotObject = GetObject(slotId);

            InventorySlot inventorySlot = InventorySlot.CreateInventorySlot(slotObject);
            _inventorySlots.TryAdd(slotId, inventorySlot);
        }
    }

    private void SetQuickSlotItem()
    {
        for (int slotId = QUICK_SLOT_START; slotId < QUICK_SLOT_END; slotId++)
        {
            InventoryItem inventoryItem = inventoryDataManager.GetInventoryItem(slotId);
            if (_inventorySlots.TryGetValue(slotId, out InventorySlot inventorySlot))
            {
                inventorySlot.SetSlotItem(inventoryItem);
            }
        }
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        InitSlot();
        SetQuickSlotItem();
    }

    private void Update() {}

    private void OnEnable()
    {
        Managers.Input.KeyAction -= OnKeyboardEvent;
        Managers.Input.KeyAction += OnKeyboardEvent;

        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
        inventoryDataManager.OnInventoryChanged += UpdateInventoryUI;
    }

    private void OnDisable()
    {
        Managers.Input.KeyAction -= OnKeyboardEvent;
        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
    }
}
