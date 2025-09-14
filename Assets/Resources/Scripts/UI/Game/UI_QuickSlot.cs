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

    private List<RectTransform> _quickSlotRects = new List<RectTransform>();
    private const int QUICK_SLOT_START = 0;
    private const int QUICK_SLOT_END = 9;
    private RectTransform _selectSlotRect;

    private void SetCurrentQuickSlot(int quickSlotId)
    {
        RectTransform rect = _quickSlotRects[quickSlotId];
        if (rect == null)
        {
            return;
        }

        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_selectSlotRect.parent, rect);
        inventoryDataManager.CurrentQuickSlotId = quickSlotId;

        _selectSlotRect.anchoredPosition = bounds.center;
    }

    private void SetInitialQuickSlot()
        => SetCurrentQuickSlot(inventoryDataManager.CurrentQuickSlotId);

    private void OnKeyboardEvent(Define.KeyBoardEvent keyEvent)
    {
        if (!(keyEvent >= Define.KeyBoardEvent.Num1 && keyEvent <= Define.KeyBoardEvent.Num9))
        {
            return;
        }

        int quickSlotId = (int)keyEvent - (int)Define.KeyBoardEvent.Num1;

        if (inventoryDataManager.CurrentQuickSlotId != quickSlotId)
        {
            SetCurrentQuickSlot(quickSlotId);
        }
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
    }

    private void GetUIElements()
    {
        _selectSlotRect = GetObject((int)GameObjects.SelectSlot).GetComponent<RectTransform>();
    }

    private void InitSlot()
    {
        _inventorySlots.Clear();

        for (int slotId = (int)GameObjects.QuickSlot0; slotId <= (int)GameObjects.QuickSlot8; slotId++)
        {
            GameObject slotObject = GetObject(slotId);
            if (slotObject == null)
            {
                continue;
            }

            InventorySlot inventorySlot = InventorySlot.CreateInventorySlot(slotObject);
            _inventorySlots.TryAdd(slotId, inventorySlot);
            _quickSlotRects.Add(slotObject.GetComponent<RectTransform>());
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
    private void Update() { }

    private void OnEnable()
    {
        Managers.Input.KeyAction -= OnKeyboardEvent;
        Managers.Input.KeyAction += OnKeyboardEvent;

        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
        inventoryDataManager.OnInventoryChanged += UpdateInventoryUI;

        Canvas.willRenderCanvases -= SetInitialQuickSlot;
        Canvas.willRenderCanvases += SetInitialQuickSlot;
    }

    private void OnDisable()
    {
        Managers.Input.KeyAction -= OnKeyboardEvent;
        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
        Canvas.willRenderCanvases -= SetInitialQuickSlot;
    }
}
