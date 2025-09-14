using System;
using System.Collections;
using System.Collections.Generic;
using Data.Date;
using Data.Game;
using Data.Inventory;
using Data.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory : UI_BaseInventory
{
    public enum Buttons
    {
        ExitButton,
        TrashButton,
        ExpandButton,
        DownsizeButton
    }

    public enum Texts
    {
        ItemName,
        PriceText,
        ItemLore,
        GoldText,
        FarmNameText,
        DateText
    }

    public enum GameObjects
    {
        InventoryScrollView,
        InventoryContent,
    }

    public enum Images
    {
        InfoSlotItemImage,
        GoldIcon,
    }

    private GameObject _contents;
    private RectTransform _contentsRectTransform;
    private RectTransform _scrollRectTransform;
    private Button _expandButton;
    private Button _downsizeButton;
    private TextMeshProUGUI _goldText;
    private TextMeshProUGUI _farmNameText;
    private TextMeshProUGUI _itemPriceText;
    private TextMeshProUGUI _itemLoreText;
    private TextMeshProUGUI _itemNameText;
    private TextMeshProUGUI _dateText;
    private Image _infoSlotItemImage;
    private Image _infoSlotGoldIcon;
    private bool _isExpanded = false;
    private float _contentsYPos = 0.0f;
    private float _originalScrollWidth;
    private float _originalContentsHeight;

    private void OnSlotClicked(PointerEventData data)
    {
        int clickedSlotId = GetSlotId(data.pointerClick.name);

        if (clickedSlotId == INVALID_SLOT_ID) return;

        if (_selectedSlotId == SLOT_NOT_SELECTED)
        {
            InventoryItem inventoryItem = inventoryDataManager.GetInventoryItem(clickedSlotId);
            if (inventoryItem == null) return;

            if (data.button == PointerEventData.InputButton.Right)
            {
                if (inventoryItem.quantity > 1) ShowSplitUI(inventoryItem, clickedSlotId);
                return;
            }

            if (!_inventorySlots.TryGetValue(clickedSlotId, out InventorySlot slot)) return;

            _selectedSlotId = clickedSlotId;
            slot.SetSlotItem(inventoryItem);
            InitDragItem(inventoryItem);

            return;
        }

        SwapInventorySlot(clickedSlotId);
    }

    private void OnSlotEntered(PointerEventData data)
    {
        int slotId = GetSlotId(data.pointerEnter.name);
        InventoryItem inventoryItem = inventoryDataManager.GetInventoryItem(slotId);

        if (inventoryItem == null)
        {
            return;
        }

        Item item = Managers.Data.GameDataManager.GetItemData(inventoryItem.itemCode);
        if (item == null)
        {
            return;
        }

        _infoSlotGoldIcon.color = Color.white;
        _itemPriceText.text = $"팔 때 : {item.sellingCost:N0}";

        _infoSlotItemImage.sprite = Managers.Resource.LoadItemSprite(item.itemCode);
        _infoSlotItemImage.color = Color.white;

        _itemLoreText.text = $"{item.itemLore}";
        _itemNameText.text = $"{item.itemName}";
    }

    private void OnSlotExit(PointerEventData data)
    {
        _infoSlotGoldIcon.color = Color.clear;
        _itemPriceText.text = "";

        _infoSlotItemImage.sprite = null;
        _infoSlotItemImage.color = Color.clear;

        _itemLoreText.text = "";
        _itemNameText.text = "";
    }

    private void OnTrashButtonClicked(PointerEventData data)
    {
        if (_selectedSlotId == SLOT_NOT_SELECTED)
        {
            return;
        }

        InventoryItem inventoryItem = inventoryDataManager.GetInventoryItem(_selectedSlotId);

        if (inventoryItem == null)
        {
            return;
        }

        Managers.UI.ShowMessageBoxUI(MessageID.RemoveItem, RemoveInventoryItem);
    }

    private void RemoveInventoryItem()
    {
        inventoryDataManager.RemoveInventoryItem(_selectedSlotId);
        DeactivateDragItem();
    }

    private void OnExpandButtonClicked(PointerEventData data)
    {
        if (_isExpanded)
        {
            return;
        }

        _isExpanded = true;

        _originalScrollWidth = _scrollRectTransform.offsetMin.x;
        _scrollRectTransform.offsetMin = new Vector2(367.5f, _scrollRectTransform.offsetMin.y);

        _contentsYPos = _contentsRectTransform.anchoredPosition.y;
        _originalContentsHeight = _contentsRectTransform.rect.height;
        _contentsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 525.0f);
        _contentsRectTransform.anchoredPosition = new Vector2(_contentsRectTransform.anchoredPosition.x, 0.0f);

        _expandButton.gameObject.SetActive(false);
        _downsizeButton.gameObject.SetActive(true);
    }

    private void OnDownsizeButtonClicked(PointerEventData data)
    {
        if (!_isExpanded)
        {
            return;
        }

        _isExpanded = false;

        _scrollRectTransform.offsetMin = new Vector2(_originalScrollWidth, _scrollRectTransform.offsetMin.y);

        _contentsRectTransform.anchoredPosition = new Vector2(_contentsRectTransform.anchoredPosition.x, _contentsYPos);
        _contentsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _originalContentsHeight);

        _expandButton.gameObject.SetActive(true);
        _downsizeButton.gameObject.SetActive(false);
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
        Bind<Image>(typeof(Images));
    }

    private void GetUIElements()
    {
        _farmNameText = GetText((int)Texts.FarmNameText);
        _goldText = GetText((int)Texts.GoldText);
        _itemNameText = GetText((int)Texts.ItemName);
        _itemPriceText = GetText((int)Texts.PriceText);
        _itemLoreText = GetText((int)Texts.ItemLore);
        _dateText = GetText((int)Texts.DateText);

        _contents = GetObject((int)GameObjects.InventoryContent);
        _contentsRectTransform = _contents.GetComponent<RectTransform>();
        _scrollRectTransform = GetObject((int)GameObjects.InventoryScrollView).GetComponent<RectTransform>();

        _infoSlotItemImage = GetImage((int)Images.InfoSlotItemImage);
        _infoSlotItemImage.color = Color.clear;
        _infoSlotGoldIcon = GetImage((int)Images.GoldIcon);
        _infoSlotGoldIcon.color = Color.clear;

        _selectedItem.SetActive(false);
        _itemSplit.SetActive(false);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
        GetButton((int)Buttons.TrashButton).gameObject.BindEvent(OnTrashButtonClicked);

        _expandButton = GetButton((int)Buttons.ExpandButton);
        _downsizeButton = GetButton((int)Buttons.DownsizeButton);

        _expandButton.gameObject.BindEvent(OnExpandButtonClicked);
        _downsizeButton.gameObject.BindEvent(OnDownsizeButtonClicked);
        _downsizeButton.gameObject.SetActive(false);
    }

    private void InitSlot()
    {
        if (_contents == null)
        {
            return;
        }

        _inventorySlots.Clear();

        foreach (Transform slot in _contents.transform)
        {
            GameObject slotObject = slot.gameObject;
            int slotId = GetSlotId(slotObject.name);

            InventorySlot inventorySlot = InventorySlot.CreateInventorySlot(slotObject);
            _inventorySlots.TryAdd(slotId, inventorySlot);

            slotObject.BindEvent(OnSlotClicked);
            slotObject.BindEvent(OnSlotEntered, Define.UIEvent.PointerEnter);
            slotObject.BindEvent(OnSlotExit, Define.UIEvent.PointerExit);
        }
    }

    private void SetInfo()
    {
        PlayerData data = Managers.Data.UserDataManager.CurrentData;

        if (data == null)
        {
            return;
        }

        _farmNameText.text = $"{data.farmName} 농장";
        _goldText.text = $"{data.gold:N0}";
        _dateText.text = $"{data.date.year}년차, {data.date.season}";
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        InitSlot();
        SetInfo();
        SetAllSlotItem();
    }

    public override void Refresh()
    {
        SetAllSlotItem();
    }
}
