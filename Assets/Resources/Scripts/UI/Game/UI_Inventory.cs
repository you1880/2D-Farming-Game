using System;
using System.Collections;
using System.Collections.Generic;
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
        TrashButton
    }

    public enum Texts
    {
        ItemName,
        PriceText,
        ItemLore,
        GoldText,
        FarmNameText,
        DragItemQuantity
    }

    public enum GameObjects
    {
        InventoryContent,
        DragItem
    }

    public enum Images
    {
        InfoSlotItemImage,
        GoldIcon,
        CharacterImage
    }

    [SerializeField] Sprite _manSprite;
    [SerializeField] Sprite _womanSprite;
    private GameObject _contents;
    private TextMeshProUGUI _goldText;
    private TextMeshProUGUI _farmNameText;
    private TextMeshProUGUI _itemPriceText;
    private TextMeshProUGUI _itemLoreText;
    private TextMeshProUGUI _itemNameText;
    private Image _infoSlotItemImage;
    private Image _infoSlotGoldIcon;
    private Image _characterImage;

    private void OnSlotClicked(PointerEventData data)
    {
        int clickedSlotId = GetSlotId(data.pointerClick.name);

        if (clickedSlotId == INVALID_SLOT_ID)
        {
            return;
        }

        if (_selectedSlotId == SLOT_NOT_SELECTED)
        {
            InventoryItem inventoryItem = inventoryDataManager.GetInventoryItem(clickedSlotId);

            if (inventoryItem == null)
            {
                return;
            }

            if (!_inventorySlots.TryGetValue(clickedSlotId, out InventorySlot slot))
            {
                return;
            }

            _selectedSlotId = clickedSlotId;
            slot.SetSlotItem(inventoryItem);
            InitDragItem(inventoryItem);
        }
        else
        {
            int targetSlotId = clickedSlotId;
            SwapInventorySlot(targetSlotId);
        }
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
        _selectedSlotId = SLOT_NOT_SELECTED;
        DeactivateDragItem();
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
        _dragItemQuantity = GetText((int)Texts.DragItemQuantity);
        _itemNameText = GetText((int)Texts.ItemName);
        _itemPriceText = GetText((int)Texts.PriceText);
        _itemLoreText = GetText((int)Texts.ItemLore);

        _contents = GetObject((int)GameObjects.InventoryContent);
        _dragItem = GetObject((int)GameObjects.DragItem);
        _dragItemRect = _dragItem.GetComponent<RectTransform>();
        _dragItem.SetActive(false);

        _dragItemImage = _dragItem.GetComponent<Image>();
        _infoSlotItemImage = GetImage((int)Images.InfoSlotItemImage);
        _infoSlotItemImage.color = Color.clear;
        _infoSlotGoldIcon = GetImage((int)Images.GoldIcon);
        _infoSlotGoldIcon.color = Color.clear;
        _characterImage = GetImage((int)Images.CharacterImage);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
        GetButton((int)Buttons.TrashButton).gameObject.BindEvent(OnTrashButtonClicked);
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

        Define.PlayerGender gender = Managers.Data.UserDataManager.CurrentData.gender;
        if (gender == Define.PlayerGender.Man)
        {
            _characterImage.sprite = _manSprite;
        }
        else
        {
            _characterImage.sprite = _womanSprite;
        }
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
