using System;
using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Chest : UI_BaseInventory
{
    public enum Buttons
    {
        ExitButton
    }

    public enum Texts
    {
        DragItemQuantity
    }

    public enum GameObjects
    {
        InventoryContent,
        ChestContent,
        DragItem
    }

    private const string CHEST_SLOT_PREFIX = "ChestSlot";
    private enum Panel { Inventory, Chest };
    private Dictionary<int, InventorySlot> _chestSlots = new Dictionary<int, InventorySlot>();
    private GameObject _inventoryContents;
    private GameObject _chestContents;
    private ChestContainer _chestContainer;
    private (Panel panel, int slotId)? _selected;
    private bool _uiReady = false;
    private bool _bindChestEvent = false;
    private bool _closed;
    public event Action ClosedAction;

    public void InitChestUI(ChestContainer chestContainer)
    {
        UnbindChestEvent();

        _chestContainer = chestContainer;
        BindChestEvent();

        if (_uiReady)
        {
            SetChestSlotsItem();
        }
    }

    private void ChangeCloseState()
    {
        if (_closed)
        {
            return;
        }

        _closed = true;
        ClosedAction?.Invoke();
    }

    private void SetChestSlotsItem()
    {
        if (_chestContainer == null)
        {
            return;
        }

        foreach (var kv in _chestSlots)
        {
            int slotId = kv.Key;
            InventorySlot inventorySlot = kv.Value;

            _chestContainer.TryGet(slotId, out InventoryItem inventoryItem);
            inventorySlot.SetSlotItem(inventoryItem);
        }
    }

    private void OnSlotClicked(Panel panel, PointerEventData data)
    {
        int clickedSlotId = (panel == Panel.Inventory) ? GetSlotId(data.pointerClick.name) : GetSlotId(data.pointerClick.name, CHEST_SLOT_PREFIX);

        if (clickedSlotId == INVALID_SLOT_ID)
        {
            return;
        }

        if (_selected is null)
        {
            InventoryItem inventoryItem = null;
            if (panel == Panel.Inventory)
            {
                inventoryItem = inventoryDataManager.GetInventoryItem(clickedSlotId);
            }
            else if (panel == Panel.Chest)
            {
                _chestContainer?.TryGet(clickedSlotId, out inventoryItem);
            }

            if (inventoryItem == null)
            {
                return;
            }

            _selected = (panel, clickedSlotId);
            InitDragItem(inventoryItem);

            return;
        }

        var (fromPanel, fromSlot) = _selected.Value;
        switch (fromPanel, panel)
        {
            case (Panel.Inventory, Panel.Inventory):
                inventoryDataManager.SwapInventorySlot(fromSlot, clickedSlotId);
                break;
            case (Panel.Inventory, Panel.Chest):
                inventoryDataManager.SwapBetweenITC(_chestContainer, fromSlot, clickedSlotId);
                break;
            case (Panel.Chest, Panel.Inventory):
                inventoryDataManager.SwapBetweenCTI(_chestContainer, fromSlot, clickedSlotId);
                break;
            case (Panel.Chest, Panel.Chest):
                inventoryDataManager.SwapChestSlot(_chestContainer, fromSlot, _chestContainer, clickedSlotId);
                break;
        }

        DeactivateDragItem();
        _selected = null;
    }

    private void UpdateChestUI(int slotId)
    {
        if (_chestContainer == null)
        {
            return;
        }

        if (_chestSlots.TryGetValue(slotId, out InventorySlot inventorySlot))
        {
            _chestContainer.TryGet(slotId, out InventoryItem inventoryItem);
            inventorySlot.SetSlotItem(inventoryItem);
        }
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
    }

    private void GetUIElements()
    {
        _dragItemQuantity = GetText((int)Texts.DragItemQuantity);
        _inventoryContents = GetObject((int)GameObjects.InventoryContent);
        _chestContents = GetObject((int)GameObjects.ChestContent);
        _dragItem = GetObject((int)GameObjects.DragItem);
        _dragItemRect = _dragItem.GetComponent<RectTransform>();
        _dragItemImage = _dragItem.GetComponent<Image>();
        _dragItem.SetActive(false);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    private void BuildSlots(Transform contents, Dictionary<int, InventorySlot> slotDict, Panel panel)
    {
        slotDict.Clear();

        foreach (Transform slotTransform in contents)
        {
            GameObject slotObject = slotTransform.gameObject;
            int slotId = panel == Panel.Inventory ? GetSlotId(slotObject.name) : GetSlotId(slotObject.name, CHEST_SLOT_PREFIX);

            InventorySlot inventorySlot = InventorySlot.CreateInventorySlot(slotObject);
            slotDict[slotId] = inventorySlot;

            slotObject.BindEvent(evt => OnSlotClicked(panel, evt));
        }
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        BuildSlots(_inventoryContents.transform, _inventorySlots, Panel.Inventory);
        BuildSlots(_chestContents.transform, _chestSlots, Panel.Chest);
        SetAllSlotItem();

        _uiReady = true;
        if (_chestContainer != null)
        {
            SetChestSlotsItem();
        }
    }

    private void BindChestEvent()
    {
        if (_chestContainer == null || _bindChestEvent)
        {
            return;
        }

        _chestContainer.OnSlotChanged += UpdateChestUI;
        _bindChestEvent = true;
    }

    private void UnbindChestEvent()
    {
        if (_chestContainer == null || !_bindChestEvent)
        {
            return;
        }

        _chestContainer.OnSlotChanged -= UpdateChestUI;
        _bindChestEvent = false;
    }

    private void OnEnable()
    {
        _closed = false;

        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
        inventoryDataManager.OnInventoryChanged += UpdateInventoryUI;
    }

    private void OnDisable()
    {
        ChangeCloseState();
        UnbindChestEvent();
        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
    }
}
